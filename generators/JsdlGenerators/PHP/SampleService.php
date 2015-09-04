<?php
	// v1.0.0
	require_once("libs/WebServiceBase.php"); 

	class Person {
		public function __construct($fn, $ln, $age) {
			$this->firstname = $fn; 
			$this->lastname = $ln; 
			$this->age = $age; 
		}
		
		/**
		 * The first name.
		 * @var string 
		 */
		public $firstname; 
		
		/**
		 * The last name. 
		 * @var string 
		 */
		public $lastname; 
		
		/**
		 * The age in years. 
		 * @var integer 
		 */
		public $age; 
	}
	
	class SampleService extends WebServiceBase {
		/**
		 * Adds two numbers
		 * @param number $a
		 * @param number $b
		 * @return number
		 */
		public function sum($a, $b) {
			return $a + $b;
		}
		
		/**
		 * Merges two arrays. 
		 * @param string[] $a
		 * @param string[] $b
		 * @return string[]
		 */
		public function mergeArrays($a, $b) {
			return array_merge($a, $b);
		}
		
		/**
		 * Gets a persen by ID. 
		 * @param integer $id
		 * @return Person
		 */
		public function getPerson($id) {
			return new Person("Rico", "Suter", 15);
		}
	}

	$svc = new SampleService();
	echo $svc->process();
?>