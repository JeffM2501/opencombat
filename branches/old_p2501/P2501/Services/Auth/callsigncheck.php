<?php
	include_once('config.php');
	include_once('../Common/db.php');
	
	function AuthDBConnect()
	{
		global $config_authDB_hostname;
		global $config_authDB_username;
		global $config_authDB_password;
		global $config_authDB_database;

		$db = mysql_pconnect($config_authDB_hostname,$config_authDB_username,$config_authDB_password);
		if (!$db)
			die("ERROR: Unable to conenct to database.\n");
		else
			$result = mysql_select_db($config_authDB_database);

		return $result;
	}
	
	function GetInput ( $var )
	{
		 if (!array_key_exists($var,$_REQUEST))
			 return FALSE;
			 
		return Sanitize($_REQUEST[$var]);
	}

	header('Content-Type: text/plain');
	
	if (!defined('CONFIGURATION'))
		die("ERROR: Unable to load configuration.\n");
		
	$db = AuthDBConnect();

	$callsign = GetInput("name");
	if (!$callsign)
		echo "na";
	else
	{
		$query = "SELECT ID FROM characters WHERE Callsign='$callsign'";
		$result = SQLGet($query);
		if ($result)
		{
			$count = mysql_num_rows($result); 
			if ($count > 0)
				echo "na";
			else
				echo "OK";
		}
		else
			echo "OK";
	}
?>