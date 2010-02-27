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
	
	function Verify ()
	{
		$id = GetInput("id");
		$token = GetInput("token");
		
		$query = "SELECT Token FROM users WHERE ID=$id";
		$result = SQLGet($query);
		if ($result)
		{
			$count = mysql_num_rows($result); 
			if ($count > 0)
			{
				$row = mysql_fetch_array($result);
				
				echo "<html><head></head><body>";
				
				if ($row[0] == $token)
				{
					$query = "UPDATE users SET Verified=1 WHERE ID=$id";
					if (!SQLSet($query))
							echo "dberr";
					else
					{
						echo "Thanks for verifying, you should see more servers when you next log in";
					}
				}
				else
					echo "Verify failed";
					
				echo "</body></html>";
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
	}	}
	
	if (!defined('CONFIGURATION'))
		die("ERROR: Unable to load configuration.\n");
		
	$db = AuthDBConnect();
	
	$action = GetInput("action");
	
	if ($action == "verify")
		Verify();
	else
		echo "no";
		
?>