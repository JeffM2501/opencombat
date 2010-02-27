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

	$uid = GetInput("uid");
	$cid = GetInput("cid");
	$token = GetInput("token");
	$ip = GetInput("ip");
	
	$callsign = false;
	$verified = false;
	
	if ($uid && $cid && $token && $ip)
	{
		$query = "SELECT IP, Token, Verified FROM users WHERE ID=$uid";
		$result = SQLGet($query);
		if ($result)
		{
			$count = mysql_num_rows($result); 
			if ($count > 0)
			{
				$row = mysql_fetch_array($result);
				if ($row[0] == $ip)
				{
					if ($row[1] == $token)
					{
						$verified = $row[2] != 0;
						
						$query = "SELECT Callsign FROM characters WHERE ID=$cid AND UID=$uid";
						$result = SQLGet($query);
						
						if ($result)
						{
							$count = mysql_num_rows($result); 
							if ($count > 0)
							{
								$row = mysql_fetch_array($result);
								$callsign = $row[0];
							}
						}
					}
				}
			}
		}
	}
	
	if (!$callsign)
		echo "bad\r\n";
	else
	{
		echo "ok\r\n";
		if ($verified)
			echo "verified\r\n";
		else
			echo "unverified\r\n";
		echo $callsign . "\r\n";
	}
					
?>