
SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";


CREATE TABLE IF NOT EXISTS `users` (
  `ID` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `PassHash` tinytext,
  `EMail` tinytext,
  `Verified` int(1) DEFAULT NULL,
  `IP` tinytext,
  `Token` int(11) DEFAULT NULL,
  `Note` tinytext,
  `Auth` tinytext,
  `LastTime` bigint(20) default NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `ID` (`ID`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=6 ;

CREATE TABLE IF NOT EXISTS `characters` (
  `ID` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `UID` int(11) DEFAULT NULL,
  `Callsign` tinytext,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `ID` (`ID`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=6 ;

