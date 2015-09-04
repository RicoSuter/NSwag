<?php
	// v1.0.9
	class JsdlGenerator {
		static function generate($className, $target) {
			$gen = new JsdlGenerator();
			return $gen->generateEx($className, $target);
		}
		
		private $types; 
		function generateEx($className, $target) {
			$this->types = array();
			
			$operations = array();
			$class = new ReflectionClass($className);
			foreach ($class->getMethods() as $method) {
				$name = $method->getName(); 
				if ($name !== "jsdl" && $name != "callOperation" && $name != "__construct" && $method->isPublic()) {
					$parameters = array();
					$methodComment = $method->getDocComment();

					$returns = $this->getReturnType($name, $methodComment);

					foreach ($method->getParameters() as $parameter) {
						$p = $this->getParameterType($methodComment, $parameter);
						$p["name"] = $parameter->getName(); 
						$parameters[] = $p; 
					}
					
					$operations[$name] = array(
						"transport" => "post", 
						"target" => str_replace("[operation]", $name, $target),
						"envelope" => "json", 
						"contentType" => "application/json", 
						"parameters" => $parameters, 
						"returns" => $returns
					);
					
					$description = $this->getDescription($methodComment);
					if ($description !== "")
						$operations[$name]["description"] = $description;
					
					$maxAge = $this->getMaxAge($methodComment);
					if ($maxAge !== 0)
						$operations[$name]["maxAge"] = $maxAge;
					
					$contentType = $this->getContentType($methodComment);
					if ($contentType !== null)
						$operations[$name]["contentType"] = $contentType;
				}
			}
			
			$types = array();
			foreach ($this->types as $key => $value)
				$types[] = $value; 
			
			return array("operations" => $operations, "types" => $types);
		}
		
		private function getDescription($comment) {
			if ($comment !== false) {
				preg_match("/.*?\\*\\*.*?\\n.*?\\*(.*)\\n/", $comment, $arr);
				return isset($arr[1]) && (!isset(trim($arr[1])[0]) || trim($arr[1])[0] != "@") ? trim($arr[1]) : ""; 
			}
			return ""; 
		}
		
		private function getReturnType($name, $methodComment) {
			if ($methodComment !== false) {
				preg_match("/\\@return (.*?)\\s/", $methodComment, $arr);
				if (isset($arr[1]))
					return $this->convertType($arr[1]); 
				else {
					preg_match("/\\@returnType ((.|[\\t\\r\\n\\v\\f])*?)(\\*\\/|\\@)/", $methodComment, $arr);
					if (isset($arr[1])) {
						$type = json_decode(str_replace("*", "", $arr[1])); 
						if ($type !== null)
							return $type; 
						die("Error converting returnType on operation '".$name."'");
					}
				}
			}
			return $this->convertType("object"); 
		}
		
		private function getMaxAge($methodComment) {
			if ($methodComment !== false) {
				preg_match("/\\@maxAge (.*?)\\s/", $methodComment, $arr);
				return isset($arr[1]) ? (int)$arr[1] : 0; 
			}
			return 0; 
		}
		
		private function getContentType($methodComment) {
			if ($methodComment !== false) {
				preg_match("/\\@contentType (.*?)\\s/", $methodComment, $arr);
				return isset($arr[1]) ? $arr[1] : null; 
			}
			return null; 
		}
		
		private function getPropertyType($propertyCommnet) {
			if ($propertyCommnet !== false) {
				preg_match("/\\@var (.*?)\\s/", $propertyCommnet, $arr);
				return $this->convertType(isset($arr[1]) ? $arr[1] : "object"); 
			}
			return $this->convertType("object"); 
		}
		
		/**
		 * @param $comment
		 * @param ReflectionParameter $parameter
		 * @return
		 */
		private function getParameterType($comment, ReflectionParameter $parameter) {
			$class = $parameter->getClass();
			if ($class !== null)
				return $this->convertType($class->getName());
			else {
				if ($comment !== false) {
					$name = $parameter->getName();
					preg_match("/\\@param (.*?) \\$$name\\s/", $comment, $arr);
					return $this->convertType(isset($arr[1]) ? $arr[1] : "object"); 
				} else
					return $this->convertType("object"); 
			}			
		}
		
		private function convertType($type) {
			if (strpos($type, "[]") > 0) {
				return array(
					"type" => "array", 
					"items" => $this->convertType(substr($type, 0, strlen($type) - 2))
				); 
			}
			
			if (class_exists($type)) {
				if (!isset($this->types[$type]))
					$this->generateType($type);
			}				
			
			switch ($type) {
				case "float": 
				case "decimal": 
				case "double": 
					$type = "number";
					break;
					
				case "integer": 
				case "int": 
					$type = "integer";
					break;
			}
			
			$result = array("type" => $type); 
			if ($type !== "object")
				$result["required"] = true; 
			return $result; 
		}
		
		private function generateType($className) {
			$type = array(
				"type" => "object", 
				"name" => $className
			);
			
			$this->types[$className] = array();   
			
			$class = new ReflectionClass($className);
			foreach ($class->getProperties() as $property) {
				if ($property->isPublic()) {
					$name = $property->getName(); 
					$propertyComment = $property->getDocComment();
					if (strpos(strtolower($propertyComment), "@jsdlignore") === false) {
						$p = $this->getPropertyType($propertyComment);
						$desc = $this->getDescription($propertyComment);
						if ($desc !== "")
							$p["description"] = $desc;
						$type["properties"][$name] = $p;
					}
				}
			}
			
			$this->types[$className] = $type;  
		}
	}
?>
