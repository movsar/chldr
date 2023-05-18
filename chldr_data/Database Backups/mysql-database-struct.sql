CREATE DATABASE  IF NOT EXISTS `u1072762_chldr` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `u1072762_chldr`;
-- MySQL dump 10.13  Distrib 8.0.32, for Win64 (x86_64)
--
-- Host: 165.22.89.128    Database: u1072762_chldr
-- ------------------------------------------------------
-- Server version	8.0.33-0ubuntu0.22.04.2

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `changesets`
--

DROP TABLE IF EXISTS `changesets`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `changesets` (
  `changeset_id` bigint NOT NULL AUTO_INCREMENT,
  `user_id` varchar(40) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `record_id` varchar(40) NOT NULL,
  `record_type` int NOT NULL,
  `record_changes` text NOT NULL,
  `operation` int NOT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`changeset_id`),
  KEY `fk_changesets_user_id_idx` (`user_id`),
  CONSTRAINT `fk_changesets_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=58 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `entry`
--

DROP TABLE IF EXISTS `entry`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `entry` (
  `entry_id` varchar(40) CHARACTER SET utf8mb3 NOT NULL,
  `user_id` varchar(40) CHARACTER SET utf8mb3 NOT NULL,
  `source_id` varchar(40) CHARACTER SET utf8mb3 NOT NULL,
  `type` int NOT NULL DEFAULT '0',
  `rate` int NOT NULL DEFAULT '0',
  `raw_contents` varchar(1500) CHARACTER SET utf8mb3 DEFAULT NULL,
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`entry_id`),
  KEY `fk_entry_user_id` (`user_id`),
  KEY `fk_entry_source_id` (`source_id`),
  CONSTRAINT `fk_entry_source_id` FOREIGN KEY (`source_id`) REFERENCES `source` (`source_id`) ON UPDATE CASCADE,
  CONSTRAINT `fk_entry_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `image`
--

DROP TABLE IF EXISTS `image`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `image` (
  `image_id` varchar(40) CHARACTER SET utf8mb3 NOT NULL,
  `user_id` varchar(40) CHARACTER SET utf8mb3 DEFAULT NULL,
  `entry_id` varchar(40) CHARACTER SET utf8mb3 NOT NULL,
  `file_name` varchar(250) CHARACTER SET utf8mb3 DEFAULT NULL,
  `rate` int NOT NULL,
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`image_id`),
  KEY `fk_image_entry_id` (`entry_id`),
  KEY `fk_image_user_id` (`user_id`),
  CONSTRAINT `fk_image_entry_id` FOREIGN KEY (`entry_id`) REFERENCES `entry` (`entry_id`) ON UPDATE CASCADE,
  CONSTRAINT `fk_image_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `language`
--

DROP TABLE IF EXISTS `language`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `language` (
  `language_id` varchar(40) NOT NULL,
  `user_id` varchar(40) DEFAULT NULL,
  `name` varchar(40) NOT NULL,
  `code` varchar(40) NOT NULL,
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`language_id`),
  KEY `fk_language_user_id` (`user_id`),
  CONSTRAINT `fk_language_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `phrase`
--

DROP TABLE IF EXISTS `phrase`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `phrase` (
  `phrase_id` varchar(40) CHARACTER SET utf8mb3 NOT NULL,
  `entry_id` varchar(40) CHARACTER SET utf8mb3 NOT NULL,
  `content` varchar(20000) CHARACTER SET utf8mb3 NOT NULL,
  `notes` varchar(1500) CHARACTER SET utf8mb3 DEFAULT NULL,
  PRIMARY KEY (`phrase_id`),
  UNIQUE KEY `entry_id_UNIQUE` (`entry_id`),
  CONSTRAINT `fk_phrase_user_id` FOREIGN KEY (`entry_id`) REFERENCES `entry` (`entry_id`) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `query`
--

DROP TABLE IF EXISTS `query`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `query` (
  `query_id` varchar(40) CHARACTER SET utf8mb3 NOT NULL,
  `user_id` varchar(40) CHARACTER SET utf8mb3 NOT NULL,
  `content` varchar(500) CHARACTER SET utf8mb3 NOT NULL,
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`query_id`),
  KEY `fk_query_user_id` (`user_id`),
  CONSTRAINT `fk_query_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sound`
--

DROP TABLE IF EXISTS `sound`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sound` (
  `sound_id` varchar(40) CHARACTER SET utf8mb3 NOT NULL,
  `user_id` varchar(40) CHARACTER SET utf8mb3 NOT NULL,
  `entry_id` varchar(40) CHARACTER SET utf8mb3 NOT NULL,
  `file_name` varchar(250) CHARACTER SET utf8mb3 NOT NULL,
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`sound_id`),
  KEY `fk_sound_entry_id` (`entry_id`),
  KEY `fk_sound_user_id` (`user_id`),
  CONSTRAINT `fk_sound_entry_id` FOREIGN KEY (`entry_id`) REFERENCES `entry` (`entry_id`) ON UPDATE CASCADE,
  CONSTRAINT `fk_sound_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `source`
--

DROP TABLE IF EXISTS `source`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `source` (
  `source_id` varchar(40) CHARACTER SET utf8mb3 NOT NULL,
  `user_id` varchar(40) CHARACTER SET utf8mb3 DEFAULT NULL,
  `name` varchar(200) CHARACTER SET utf8mb3 NOT NULL,
  `notes` varchar(500) CHARACTER SET utf8mb3 DEFAULT NULL,
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`source_id`),
  KEY `fk_source_user_id` (`user_id`),
  CONSTRAINT `fk_source_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `text`
--

DROP TABLE IF EXISTS `text`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `text` (
  `text_id` varchar(40) CHARACTER SET utf8mb3 NOT NULL,
  `entry_id` varchar(40) CHARACTER SET utf8mb3 NOT NULL,
  `content` varchar(20000) CHARACTER SET utf8mb3 NOT NULL,
  `notes` varchar(1500) CHARACTER SET utf8mb3 DEFAULT NULL,
  PRIMARY KEY (`text_id`),
  UNIQUE KEY `entry_id_UNIQUE` (`entry_id`),
  CONSTRAINT `fk_text_entry_id` FOREIGN KEY (`entry_id`) REFERENCES `entry` (`entry_id`) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tokens`
--

DROP TABLE IF EXISTS `tokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tokens` (
  `token_id` varchar(40) CHARACTER SET utf8mb3 NOT NULL,
  `user_id` varchar(40) CHARACTER SET utf8mb3 NOT NULL,
  `type` int DEFAULT '0',
  `value` varchar(300) CHARACTER SET utf8mb3 DEFAULT NULL,
  `expires_in` datetime DEFAULT NULL,
  `created_at` datetime DEFAULT NULL,
  PRIMARY KEY (`token_id`),
  KEY `fk_tokens_user_id_idx` (`user_id`),
  CONSTRAINT `fk_tokens_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `translation`
--

DROP TABLE IF EXISTS `translation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `translation` (
  `translation_id` varchar(40) CHARACTER SET utf8mb3 NOT NULL,
  `language_id` varchar(40) CHARACTER SET utf8mb3 NOT NULL,
  `entry_id` varchar(40) CHARACTER SET utf8mb3 NOT NULL,
  `user_id` varchar(40) CHARACTER SET utf8mb3 NOT NULL,
  `content` varchar(10000) CHARACTER SET utf8mb3 NOT NULL,
  `raw_contents` varchar(10000) CHARACTER SET utf8mb3 NOT NULL,
  `notes` varchar(1000) CHARACTER SET utf8mb3 DEFAULT NULL,
  `rate` int NOT NULL DEFAULT '0',
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`translation_id`),
  KEY `fk_translation_language_id` (`language_id`),
  KEY `fk_translation_entry_id` (`entry_id`),
  KEY `fk_translation_user_id` (`user_id`),
  CONSTRAINT `fk_translation_entry_id` FOREIGN KEY (`entry_id`) REFERENCES `entry` (`entry_id`) ON UPDATE CASCADE,
  CONSTRAINT `fk_translation_language_id` FOREIGN KEY (`language_id`) REFERENCES `language` (`language_id`) ON UPDATE CASCADE,
  CONSTRAINT `fk_translation_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `user_id` varchar(40) CHARACTER SET utf8mb3 NOT NULL,
  `email` varchar(200) CHARACTER SET utf8mb3 DEFAULT NULL,
  `password` varchar(250) CHARACTER SET utf8mb3 DEFAULT NULL,
  `rate` int NOT NULL DEFAULT '0',
  `image_path` varchar(250) CHARACTER SET utf8mb3 DEFAULT NULL,
  `first_name` varchar(100) CHARACTER SET utf8mb3 DEFAULT NULL,
  `last_name` varchar(100) CHARACTER SET utf8mb3 DEFAULT NULL,
  `patronymic` varchar(100) CHARACTER SET utf8mb3 DEFAULT NULL,
  `is_moderator` tinyint DEFAULT NULL,
  `user_status` tinyint DEFAULT NULL,
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime DEFAULT NULL,
  PRIMARY KEY (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `word`
--

DROP TABLE IF EXISTS `word`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `word` (
  `word_id` varchar(40) CHARACTER SET utf8mb3 NOT NULL,
  `entry_id` varchar(40) CHARACTER SET utf8mb3 NOT NULL,
  `content` varchar(10000) CHARACTER SET utf8mb3 NOT NULL,
  `notes` varchar(1500) CHARACTER SET utf8mb3 DEFAULT NULL,
  `part_of_speech` int DEFAULT NULL,
  `additional_details` varchar(999) DEFAULT NULL,
  PRIMARY KEY (`word_id`),
  UNIQUE KEY `entry_id_UNIQUE` (`entry_id`),
  CONSTRAINT `fk_word_entry_id` FOREIGN KEY (`entry_id`) REFERENCES `entry` (`entry_id`) ON UPDATE CASCADE,
  CONSTRAINT `word_chk_1` CHECK (json_valid(`additional_details`))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2023-05-18 13:03:58
