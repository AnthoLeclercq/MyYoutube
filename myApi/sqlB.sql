-- CREATE DATABASE IF NOT EXISTS `myapi`
-- USE `myapi`;

CREATE TABLE IF NOT EXISTS `comment` (
  `id` int(11) NOT NULL,
  `body` longtext,
  `USER_id` int(11) NOT NULL,
  `video_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS `token` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `USERId` int(11) NOT NULL,
  `Token` varchar(100) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `USERId` (`USERId`),
  CONSTRAINT `FK_token_USER` FOREIGN KEY (`USERId`) REFERENCES `USER` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;