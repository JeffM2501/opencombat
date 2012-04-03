<?php  	
	include_once("utils.php");
	
	function AddHost( $db )
	{
		$key = Request("key");
		if (!$key)
			DieWithError("addhost;Error=No Key");
			
		$keyID =  GetDBFieldForKey("Secret", $key, "HostKeys", "ID" );
		if (!$keyID)
			DieWithError("addhost;Error= Unknown Key");
			
		$host = GetDBFieldForID( $keyID, "HostKeys", "Host" );
		
		if (!$host)
			DieWithError("addhost;Error=No Host");
			
		$remoteHost = $_SERVER['REMOTE_ADDR'];
		if ($remoteHost != $host)
			DieWithError("addhost;Error=Invalid Host Requst " . $remoteHost);
			
		// see if we have a record with this host and key
		$hosts = FieldsFromQuery("SELECT ID FROM Hosts WHERE Host='$host' AND Secret='$key'");
		if ($hosts)
			$serverID = $hosts[0]['ID'];
		else
		{
			$r = rand();
			InsertDBField( "Hosts", "Host", $r );
			$serverID = GetDBFieldForKey ( "Host", $r, "Hosts", "ID" );
			
			UpdateDBFieldForID ( $serverID, "Hosts", "Host", $host );
			UpdateDBFieldForID ( $serverID, "Hosts", "Secret", $key );
		}
		
		UpdateDBFieldForID ( $serverID, "Hosts", "Instances", Request("instances") );
		UpdateDBFieldForID ( $serverID, "Hosts", "Players", Request("players") );
		
		UpdateDBFieldForID ( $serverID, "Hosts", "Max_Instances", Request("max_instances") );
		UpdateDBFieldForID ( $serverID, "Hosts", "Max_Players", Request("max_players") );
		UpdateDBFieldForID ( $serverID, "Hosts", "Last_Heartbeat", NOW_DATETIME() );
			
		echo "addhost;OK;ID=" . $serverID;
	}
	
	$db = ConnectToDB();
	
	$action = Request("action");
	if (!$action)
		DieWithError("No Action");
		
	if ($action === "addhost")
		AddHost($db);
?>