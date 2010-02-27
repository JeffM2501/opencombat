<?php
	include_once('config.php');
	include_once('../Common/db.php');
	
	function ListDBConnect()
	{
		global $config_listDB_hostname;
		global $config_listDB_username;
		global $config_listDB_password;
		global $config_listDB_database;

		$db = mysql_pconnect($config_listDB_hostname,$config_listDB_username,$config_listDB_password);
		if (!$db)
			die("ERROR: Unable to conenct to database.\n");
		else
			$result = mysql_select_db($config_listDB_database);

		return $result;
	}
	
	function ViewList()
	{
		$query = "SELECT ID, Hostname, ServerName, Description, Groupname FROM list";
		$result = SQLGet($query);
		
		$count = mysql_num_rows($result); 
		echo $count . "\r\n";
		for ($i = 0; $i < $count; $i += 1)
		{
			$row = mysql_fetch_array($result);
			echo $row[1] ."\t". $row[2] ."\t". $row[3] ."\t". $row[4] ."\r\n";
		}
	}
	
	function GetInput ( $var )
	{
		 if (!array_key_exists($var,$_REQUEST))
			 return FALSE;
			 
		return Sanitize($_REQUEST[$var]);
	}
	
	function AddHost()
	{
		$now = time();

		$host = GetInput("host");
		$name = GetInput("name");
		$description = GetInput("desc");
		$key = GetInput("key");
		$type = GetInput("type");
	
		$ip = $_SERVER['REMOTE_ADDR'];
		$port = explode(":",$host);
		if (count($port) > 1)
			$port = $port[1];
		else
		{
			$port = 2501;
			$host = $host . ":2501";
		}
		
		if (!$name)
			$name = $host;
			
		if (!$key)
			$group = "Community";
		else
		{
			$query = "SELECT GroupName FROM authKeys WHERE AuthKey='$key'";
			$groupName = GetQueryResult(SQLGet($query));
			if (!$groupName)
				$group = "Community";
			else
				$group = $groupName;
		}
		
		if (!$type || $type != "verified")
			$group = $group . " Open";
		else
			$group = $group . " Registered";

		if (!$description)
			$description = "NULL";
			
		if (!$host)
		{
			echo "err";
			return;
		}
		
		// find this server
		
		$query = "SELECT ID FROM list WHERE IP='$ip' AND Hostport='$port'";
		$id = GetQueryResult(SQLGet($query));
		if ($id)
		{				
				$query = "UPDATE list SET Hostname='$host', IP='$ip', HostPort='$port', ServerName='$name', Description='$description', Groupname='$group', TimeStamp=$now WHERE ID=$id";
				if (!SQLSet($query))
				{
						echo "dberr";
						return;
				}
				echo $id;
				return;
		}
				
		$random = rand();
		
		$query = "INSERT INTO list (Hostname, IP, HostPort, ServerName, Description, Groupname, TimeStamp) VALUES ('$host', '$ip', '$port', '$name', '$description', '$random', $now)";
		if (!SQLSet($query))
		{
				echo "dberr";
				return;
		}
		
		$id = GetDBFieldForKey ( "Groupname", $random, "list", "ID" );
		if (!$id)
		{
				echo "dberr";
				return;
		}
		
		SetDBFieldForKey("ID", $id, "list", "Groupname", $group);
		 
		echo $id;
	}
	
	function UpdateHost()
	{
		$id = GetInput("id");
		$ip = $_SERVER['REMOTE_ADDR'];	
		
		if (!$id)
		{
			 echo "err";
			 return;
		}
		
		$hostIP = GetDBFieldForKey ( "ID", $id, "list", "IP" );

		if (!$hostIP || $hostIP != $ip)
		{
			 echo "err";
			 return;
		}
		
		SetDBFieldForKey("ID", $id, "list", "TimeStamp", time());
	
		echo $id;
	}
	
	function RemoveHost()
	{
		$id = GetInput("id");
		$ip = $_SERVER['REMOTE_ADDR'];	
		
		if (!$id)
		{
			 echo "err";
			 return;
		}
		
		$hostIP = GetDBFieldForKey ( "ID", $id, "list", "IP" );

		if (!$hostIP || $hostIP != $ip)
		{
			 echo "err";
			 return;
		}
		
		$query = "DELETE FROM list WHERE ID=$id";
		
		if (!SQLSet($query))
			echo "err";
		else
			echo "ok";
	}
	
	function CheckOlds()
	{
		$then = time()-18000;
		$query = "DELETE FROM list WHERE TimeStamp < $then";
		SQLSet($query);
	}
	
	header('Content-Type: text/plain');
	
	if (!defined('CONFIGURATION'))
		die("ERROR: Unable to load configuration.\n");
		
	$db = ListDBConnect();

	 if (array_key_exists('action',$_REQUEST))                         
		$action = $_REQUEST['action'];
	else
		$action = '';
		
	if ($action == 'viewlist')
		ViewList();
	else if ($action == 'addhost')
		AddHost();
	else if ($action == 'updatehost')
		UpdateHost();
	else if ($action == 'removehost')
		RemoveHost();
		
		CheckOlds();
?>