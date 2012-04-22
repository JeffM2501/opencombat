<?php

include_once("config.php");

function BuildHashTable ($file, $localPath, $rootPath, $log)
{
	$subs = scandir($rootPath);
	
	foreach ($subs as $sub)
	{
		if ($sub != ".." && $sub != ".")
		{
			$thisFullPath = $rootPath . $sub;
			$thisLocalPath = $localPath  . $sub;

			if (is_dir($thisFullPath))
			{
				BuildHashTable($file,$thisLocalPath."/",$thisFullPath."/",$log);
			}
			else
			{
				$hash = md5_file($thisFullPath);
				fwrite($file, $hash . ";" . $thisLocalPath . ";". "Unknown\r\n");
				
				if ($log)
					echo "$hash $thisLocalPath </br>\r\n";
			}
		}
	}
}

global $DataRootLocation;
global $HashLocation;
global $ForceBuildKey;

$action = "";
if (isset($_REQUEST['action']))
	$action = $_REQUEST['action'];

if ($action == "getfile")
{
	$path = "";
	if (isset($_REQUEST['file']))
		$path = $_REQUEST['file'];
		
	if (strstr($path,"..") !== FALSE || strstr($path,"./") !== FALSE || strstr($path,".\\") !== FALSE)
		exit();
	
	$fileName = $DataRootLocation . $path;
	readfile($fileName);
}
else
{
	$forceBuild = FALSE;
	
	if($action == "forcebuild")
	{
		if (isset($_REQUEST['key']))
		{
			if ($_REQUEST['key'] == $ForceBuildKey)
				$forceBuild = TRUE;
		}
	}
	// check the hashfile, if not rebuild it
	if (!file_exists($HashLocation) || $forceBuild)
	{
		$log = false;
		if ($action != "gethash")
			$log = true;
		
		if ($log)
			echo "Building Hash</br>\r\n";
			
		$file = fopen($HashLocation,"w");
		BuildHashTable($file,"",$DataRootLocation,$log);
		
		fclose($file);
		if ($log)
			echo "Hash Complete</br>\r\n";
	}
	
	if ($action == "gethash")
		readfile($HashLocation);
}


?>