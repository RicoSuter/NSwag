<?php
	// v1.0.9
	@include_once("JsdlGenerator.php"); 

	class JsdlSecurityException extends Exception {}
	class JsdlParameterException extends Exception {}
	class JsdlOperationNotFoundException extends Exception {}
	
	class WebServiceBase {
		public function __construct($target = null) {
			if ($target === null)
				$this->target = get_class($this).".php&operation=[operation]";
			else
				$this->target = $target; 
		}
		
		protected function isUserInRole($role) {
			throw new Exception("Method isUserInRole is not implemented on JSDL service class!"); // override method
		}
		
		function jsdl() {
			if (class_exists("JsdlGenerator"))
				return JsdlGenerator::generate(get_class($this), $this->target);
			return "JsdlGen_class_not_found";
		}
	
		function process($operation = null, $inputJson = null, $logFile = null, $prettyPrint = null) {
			if ($operation === null) {
				if (isset($_GET["operation"]))
					$operation = $_GET["operation"];
				else
					return "operation_not_set";
			}
			
			if ($prettyPrint === null)
				$prettyPrint = isset($_GET["pretty"]);
			
			// load parameters
			if ($inputJson === null) 
				$inputJson = file_get_contents("php://input");
						
			if ($inputJson === "") 
				$inputJson = "[]";
			
			$loggingAllowed = (!isset($this->notLoggedOperations) || !in_array($operation, $this->notLoggedOperations)); 
			if ($logFile !== null && $loggingAllowed) {
				file_put_contents($logFile, 
					date("m/d/Y h:i:s a", time())."\n".
					$operation."\n".
					(strlen($inputJson) < 1024 * 2 ? $inputJson : "input_too_big")."\n", 
					FILE_APPEND);
			}
						
			try {
				// check permissions
				$method = new ReflectionMethod($this, $operation);
				if ($method == null)
					throw new JsdlOperationNotFoundException("Operation '".$operation."' not found!");
				
				$comment = $method->getDocComment();
				if ($comment !== false) {
					if (strpos(strtolower($comment), "@jsdlignore") !== false)
						throw new JsdlOperationNotFoundException("Operation '".$operation."' not found!");
					
					preg_match("/\\@role (.*?)\\s/", $comment, $arr);
					$roles = isset($arr[1]) ? explode(",", str_replace(" ", "", $arr[1])) : array(); 
					foreach ($roles as $role) {
						if (!$this->isUserInRole($role))
							throw new JsdlSecurityException("User not in role '".$role."'; required for operation '".$operation."')!");
					}
				}
				
				// validate parameters
				$i = 0; 
				$parameters = json_decode($inputJson, true); 
				foreach ($method->getParameters() as $parameter) {
					$type = $this->getParameterType($comment, $parameter);
					$this->checkParameterType($parameter->getName(), $type, $parameters[$i]);
					$i++; 
				}
				
				// call operation
				$result = call_user_func_array(array($this, $operation), $parameters);
			} catch (Exception $e) {
				$protocol = (isset($_SERVER['SERVER_PROTOCOL']) ? $_SERVER['SERVER_PROTOCOL'] : 'HTTP/1.0');
                header($protocol.' 500 500');
				$result = array("code" => get_class($e), "message" => $e->getMessage());				
			}
			
			// process result
			if ($result === null)
				$output = "null";
			else
				$output = json_encode($result, $prettyPrint ? JSON_PRETTY_PRINT : 0);
			
			if ($logFile !== null && strlen($output) < 1024 * 2 && $loggingAllowed)
				file_put_contents($logFile, $output."\n\n", FILE_APPEND);
			
			header('Content-Type: application/json; charset=utf-8');
			return $output; 
		}
		
		/**
		 * @param string $comment
		 * @param ReflectionParameter $parameter
		 * @return
		 */
		private function getParameterType($comment, ReflectionParameter $parameter) {
			$class = $parameter->getClass();
			if ($class !== null)
				return $class->getName();
			else {
				if ($comment !== false) {
					$name = $parameter->getName();
					preg_match("/\\@param (.*?) \\$$name\\s/", $comment, $arr);
					return isset($arr[1]) ? $arr[1] : "object"; 
				} else
					return "object"; 
			}	
		}
		
		/**
		 * @param string $name
		 * @param string $type
		 * @param mixed $value
		 */
		private function checkParameterType($name, $type, $value) {
			if ($value === null) // TODO check required attribute
				return;
			
			if ($type[strlen($type) - 2] == "[" && $type[strlen($type) - 1] == "]") { // is array
				if (!is_array($value))
					throw new JsdlParameterException("data validation error");
				
				$type = substr($type, 0, strlen($type) - 2);
				foreach ($value as $val)
					$this->checkParameterType($name, $type, $val);				
				return; 
			}
			
			switch ($type) { 
				case "int": 
				case "integer": 
					if (intval($value) != $value)
						throw new JsdlParameterException("data validation error - parameter '$name' of type '$type'");
					break; 
				
				case "number": 
					if (!is_numeric($value))
						throw new JsdlParameterException("data validation error - parameter '$name' of type '$type'");
					break;
				
				case "bool": 
				case "boolean": 
					if (!is_bool($value))
						throw new JsdlParameterException("data validation error - parameter '$name' of type '$type'");
					break;
					
				// TODO check complex types
			}
		}
	}
?>