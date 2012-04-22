<?php  
	include_once("config.php");
	
	function ConnectToDB ()
	{
		global $CONFIG_DATABASE_SERVER;
		global $CONFIG_DATABASE_DATABSE;
		global $CONFIG_DATABASE_USER;
		global $CONFIG_DATABASE_PASS;

		$db = mysql_pconnect($CONFIG_DATABASE_SERVER,$CONFIG_DATABASE_USER,$CONFIG_DATABASE_PASS);
		if (!$db)
		{
			header('Content-Type: text/plain');
			die("ERROR: Unable to conenct to database.\n");
		}
		else
			$result = mysql_select_db($CONFIG_DATABASE_DATABSE);
		return $db;
	}
	
	function SQLError ( $query )
	{
		echo "SQL ERROR: " . mysql_error() . "<br>";
		echo "SQL ERROR Query: " . $query . "<br>";
	}
	
	function SQLGet ( $query )
	{
		$result = mysql_query($query);
		if (!$result && $result != 0 && mysql_num_rows($result) > 0)
			SQLError($query);
			
		return $result;
	}
	
	function SQLSet ( $query )
	{
		$result = mysql_query($query);
		if (!$result)
			SQLError($query);
			
		return TRUE;
	}
	
	function GetQueryResults ( $result, $field )
	{
		if (!$result)
			return FALSE;
			
		$list = array(); 
		$count = mysql_num_rows($result);
		for ($i = 0; $i < $count; $i += 1)
		{
			$row = mysql_fetch_array($result);
			$list[] = $row[$field];
		}
		
		return $list;
	}
	
	function GetQueryAllResults ( $result  )
	{
		if (!$result)
			return FALSE;
			
		$list = array(); 
		$count = mysql_num_rows($result);
		for ($i = 0; $i < $count; $i += 1)
		{
			$row = mysql_fetch_array($result);
			$filteredRow = array();
			foreach ($row as $key=>$value)
				$filteredRow[$key] = Unsanitize($value);	
			$list[] = $filteredRow;
		}
		return $list;
	}
	
	function FieldsFromQuery ( $query )
	{
		$results = GetQueryAllResults(SQLGet($query) );
		if (!$results)
			return FALSE;
		return $results;
	}
	
	function GetDBFieldForKey ( $keyName, $key, $table, $field )
	{
		$query = "SELECT " . $field . " FROM ". $table ." WHERE " . $keyName . "='" .$key . "'";
		
		$results = GetQueryResults(SQLGet($query),$field );
		if (!$results)
			return FALSE;
		return Unsanitize($results[0]);
	}
	
	function GetDBFieldForID ( $id, $table, $field )
	{
		return GetDBFieldForKey("ID",$id,$table,$field);
	}
	
	function UpdateDBFieldForKey ( $keyName, $key, $table, $field, $value )
	{
		$query = "UPDATE " . $table ." SET " . $field . "='" .$value."' WHERE " . $keyName ."='" .$key. "'";
		return SQLSet($query); 
	}
	
	function UpdateDBFieldForID ( $id, $table, $field, $value )
	{
		return UpdateDBFieldForKey("ID", $id, $table, $field, $value);
	}
	
	function InsertDBField ( $table, $field, $value )
	{
		$query = "INSERT INTO " . $table ."(" . $field . ") VALUES('" . $value . "')";
		return SQLSet($query); 
	}
	
	function Sanitize ( $value )
	{
		return mysql_real_escape_string(addslashes($value));	
	}
	
	function Unsanitize ( $value )
	{
		return stripslashes($value);	
	}
	
?>