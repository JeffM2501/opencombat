<?php  	
	include_once("config.php");
	include_once("db.php");
	
	function Request ($index)
	{
		if (isset($_REQUEST[$index]))
			return Sanitize($_REQUEST[$index]);
		return FALSE;
	}
	
	function DieWithError ($error)
	{
		echo $error;
		exit();	
	}
	
	function NOW_DATETIME()
	{
		return date( 'Y-m-d H:i:s');
	}
	
	function SQLTIME_TO_PHPTIME( $time )
	{
		return strtotime($time);
	}
?>