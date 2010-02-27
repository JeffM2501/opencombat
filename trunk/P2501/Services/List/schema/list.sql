
SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";

--
-- Table structure for table `list`
--

CREATE TABLE IF NOT EXISTS `list` (
  `ID` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Hostname` tinytext,
  `IP` tinytext,
  `HostPort` int(11) DEFAULT NULL,
  `ServerName` tinytext,
  `Description` tinytext,
  `ServerType` tinytext,
  `TimeStamp` bigint(20) default NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `ID` (`ID`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=6 ;


CREATE TABLE IF NOT EXISTS `authKeys` (
  `ID` bigint(20) NOT NULL AUTO_INCREMENT,
  `AuthKey` tinytext,
  `GroupName` tinytext,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `ID` (`ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1 AUTO_INCREMENT=1 ;
