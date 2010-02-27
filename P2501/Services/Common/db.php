<?php  	
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
	
	function GetQueryResult ( $result )
	{
		if ($result && mysql_num_rows($result) > 0)
		{
			$nub = mysql_fetch_array($result);
			return $nub[0];
		}
	
		return FALSE;
	}
	
	function GetDBFieldForKey ( $keyName, $key, $db, $field )
	{
		$query = "SELECT " . $field . " FROM ". $db ." WHERE " . $keyName . "='" .$key . "'";
		$results = GetQueryResults(SQLGet($query),$field );
		if (!$results)
			return FALSE;
		return Unsanitize($results[0]);
	}
	
	function GetDBFieldForID ( $id, $db, $field )
	{
		return GetDBFieldForKey("ID",$id,$db,$field);
	}
	
	function SetDBFieldForKey ( $keyName, $key, $db, $field, $value )
	{
		$query = "UPDATE " . $db ." SET " . $field . "='" .$value."' WHERE " . $keyName ."='" .$key. "'";
		return SQLSet($query); 
	}
	
	function SetDBFieldForID ( $id, $db, $field, $value )
	{
		return SetDBFieldForKey("ID", $id, $db, $field, $value);
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