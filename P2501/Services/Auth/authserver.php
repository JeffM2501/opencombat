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
	
	function CharacterExists ( $name )
	{
		$query = "SELECT ID FROM characters WHERE Callsign='$name'";
		$result = SQLGet($query);
		if ($result)
		{
			$count = mysql_num_rows($result); 
			if ($count > 0)
				return TRUE;
		}
		
		return FALSE:
	}
	
	function AccountExists ( $name )
	{
		$query = "SELECT ID FROM users WHERE Email='$name'";
		$result = SQLGet($query);
		if ($result)
		{
			$count = mysql_num_rows($result); 
			if ($count > 0)
				return TRUE;
		}
		
		return FALSE:
	}
	
	function AddUser ()
	{
		$email = GetInput("email");
		$password = GetInput("password");
		$character = GetInput("character");
		
		if (!$email || AccountExists($email))
		{
			echo "authbademail";
			return;
		}
		
		if (!$character || CharacterExists($character))
		{
			echo "authbadcallsign";
			return;
		}
		
		if (!$password )
		{
			echo "authbadpass";
			return;
		}
		
		$passhash = md5($password);
		$token = rand();
		
		$query = "INSERT INTO users (EMail, PassHash, Verified, Auth) VALUES ('$email','$passhash',0,'$token')");
		SQLSet($query);
		          
		$query = "SELECT ID FROM users WHERE Auth='$token'");
		
		$id = GetQueryResult(SQLGet($query));
		if (!$id)
		{
			echo "error";
			return;
		}

		$query = "INSERT INTO characters (UID, Callsign) VALUES ($id,'$character')");
		SQLSet($query);
		
		echo "ok";

		$from = "auth@opencombat.net";
    $subject = "Project2501 Registration";
    $body = "Thank you for registering the account $email\r\nPlease click this link http://auth.opencombat.net/webauth.php?action=verify&id=$id&token=$token to verify your account and get access to more servers\r\n";
		
		mail($email,$subject,$body);
	}
	
	function characterCount ($uid)
	{
		$query = "SELECT ID FROM characters WHERE UID=$uid");
		
		$results = SQLGet($query));
		if (!$results)
			return 0;
		return  mysql_num_rows($results);
	}
	
	function AddCharacter ()
	{
		if (!checkAuth())
		{
			echo "addcharbadnoauth";
			return;
		}
		
		$uid = GetInput("uid");
		$callsign = GetInput("callsign");

		if (CharacterExists($callsign))
		{
			echo "addcharbadcallsign";
			return;
		}
		
		if (characterCount($uid) >= 4)
		{
			echo "addcharbadctoomany";
			return;
		}
		
		$query = "INSERT INTO characters (UID, Callsign) VALUES ($uid,$callsign)";
		SQLSet($query);
		
		echo "ok";
	}
	
	function Login ()
	{
		$email = GetInput("email");
		$pass = GetInput("password");
		
		$passhash = md5($password);

		$query = "SELECT ID, PassHash FROM users WHERE EMail='$email'");
		
		$results = SQLGet($query));
		if (!$results || mysql_num_rows($results) == 0)
		{
			echo "authbadcred";
			return;
		}
		
		$row = mysql_fetch_array($results);
		if (!$row[0] || $passhash != $row[1])
		{
			echo "authbadcred";
			return;
		}
		
		$id = $row[0];
		
		
		$token = rand();
		$ip = $_SERVER['REMOTE_ADDR'];
		
		$query = String.Format("UPDATE users SET Token='$token', IP='$ip' WHERE ID=$id");
		SQLSet($query);
		
		$_SESSION['uid'] = $id;
	  $_SESSION['token'] = $token;
		$_SESSION['ip'] = $ip;

		echo "ok\r\n" . $id . "\r\n" ."token\r\n";
	}
	
	function checkAuth ()
	{
		$uid = GetInput("uid");
		$ip = $_SERVER['REMOTE_ADDR'];

		return $_SESSION['uid'] == $uid && $_SESSION['ip'] == $ip;
	}
	
	function ListCharacters()
	{
		if (!checkAuth())
		{
			echo "listbadnoauth";
			return;
		}
		
		$uid = GetInput("uid");
		$query = "SELECT ID, Callsign FROM characters WHERE UID=$uid";
		
		$results = SQLGet($query));
		if (!$results || mysql_num_rows($results) == 0)
			echo "0\r\n";
		else
		{
			$count =  mysql_num_rows($results);
			echo $count . "\r\n";
			for ($i = 0; $i < $count; $i += 1)
			{
				$row = mysql_fetch_array($results);
				echo $row[0] . "\t" . $row[1] . "\r\n";
			}
		}
		
		session_unset();
	}
	
	function RemoveCharacter()
	{
	}
	
	session_name('auth'); 
	session_start(); 
	
	if (!defined('CONFIGURATION'))
		die("ERROR: Unable to load configuration.\n");
		
	$db = AuthDBConnect();
	
	$action = GetInput("action");
	
	if ($action == "adduser")
		AddUser();
	else if ($action == "addchar")
		AddCharacter();
	else if ($action == "removechar")
		RemoveCharacter();
	else if ($action == "listchar")
		ListCharacters();
	else if ($action == "login")
		Login();
	else
		echo "no";
?>