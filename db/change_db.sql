ALTER TABLE  `video_detail` CHANGE  `file_id`  `file_id` DOUBLE NOT NULL AUTO_INCREMENT;
ALTER TABLE  `video_size_detail` CHANGE  `file_id`  `file_id` DOUBLE NOT NULL AUTO_INCREMENT;
ALTER TABLE  `video_time_division` CHANGE  `id`  `id` DOUBLE NOT NULL AUTO_INCREMENT;
ALTER TABLE  `video_time_division` CHANGE  `video_id`  `video_id` DOUBLE NOT NULL;
ALTER TABLE  `video_time_division` CHANGE  `size_id`  `size_id` DOUBLE NOT NULL;