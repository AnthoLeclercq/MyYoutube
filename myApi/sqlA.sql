CREATE TABLE IF NOT EXISTS `USER` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `USERname` varchar(50) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `Pseudo` varchar(50) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `created_at` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `email` varchar(100) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `password` varchar(200) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=40 DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS `video` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(45) NOT NULL,
  `duration` int(11) DEFAULT NULL,
  `USER_id` int(11) NOT NULL,
  `source` varchar(255) NOT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `view` int(11) NOT NULL DEFAULT '0',
  `enabled` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `fk_video_USER_idx` (`USER_id`),
  CONSTRAINT `FK_USER` FOREIGN KEY (`USER_id`) REFERENCES `USER` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS `video_format` (
  `id` int(11) NOT NULL,
  `code` varchar(45) NOT NULL,
  `uri` varchar(45) NOT NULL,
  `video_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_video_format_video1_idx` (`video_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;