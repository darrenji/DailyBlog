> 创建表

```
DROP TABLE IF EXISTS `table_name`
CREATE TABLE `table_name`(
	`id` bigint(20) NOT NULL auto_increment COMMENT 'ID',
	`column` bigint(20) NOT NULL COMMENT '',
	`column` varchar(16) NOT NUL COMMENT '',
	`column` bigint(20) NOT NULL default '0' COMMENT '',
	`column` varchar(20) default NULL COMMENT '',
	`type` smallint(6) NOT NULL default '0' COMMENT '',
	`date_time` datetime NOT NULL COMMENT '',
	`is_read` int(1) default '0' COMMENT '',
	`o_c` bit(1) default NULL COMMENT '',
	`power` decimal(16,2) default NULL COMMENT '',
	`acq_time` datetime default NULL COMMENT '',
	`updae_time` teimestamp NOT NULL default CURRENT_TIMESTARMP on update CURRENT_TIMESTAMP COMMENT ''
	PRIMARY KEY (`id`),
	UNIQUE KEY `UK_ALARM`(``,``)
	KEY `IDX_BOX_PROJECT` ('prject_code')
	CONSTRAINT `FK_RESOURCE_OPERATION_KEY` FOREIGN KEY (`RESOURCE_KEY`, `OPERATION_KEY`) REFERENCES `sys_operation` (`RESOURCE_KEY`, `OPERATION_KEY`) ON DELETE CASCADE,
	CONSTRAINT `FK_ROLE_ID` FOREIGN KEY (`ROLE_ID`) REFERENCES `sys_role` (`ROLE_ID`) ON DELEE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='备注名称'
```

> 插入数据

```
LOCK TABLES `table_name` WRITE;
INSERT INTO `table_name` VALUES 
(),
();
UNLOCK TABLES;
```

> 更新

```
update table_name t set t.column=... where t.colum=''
```

> 创建存储过程

```
DROP PROCEDURE IF EXISTS ebx.p_clean_box_data;

DELIMITER $$
CREATE PROCEDURE `p_clean_box_data`(IN `p_mac` varchar(50), IN `p_method` varchar(20), OUT `o_ret` varchar(30))
	LANGUARGE SQL
	NOT DETERMINISTIC
	CONTAINS SQL
	SQL SECURITY INVOKER
	COMMENT ''

BEGIN
	START TRANSACTION;
		DELETE FROM EBX_ALARM WHERE MAC = p_mac;
		
		IF p_method='ALL' THEN
			DELETE FROM EBX_BOX WHERE MAC = p_mac;			
		END IF;
	COMMIT;
	SET o_ret='1';
END$$
DELIMITER ;	

-- [如果是root执行的脚本，需要执行这个SQL]
-- update mysql.proc t set t.`definer`='ebxdbu@127.0.0.1' where t.db='ebx' and t.name='p_clean_box_data';
-- FLUSH PRIVILEGES;
```

> 更改表结构

```
ALTER TABLE `ebx_current`
	CHANGE COLUMN `current` `current` DECIMAL(16,3) NULL DEFAULT NULL COMMENT ''
```