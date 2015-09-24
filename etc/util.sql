SELECT OBJECT_NAME(i.object_id) AS TableName ,i.name AS TableIndexName ,
phystat.avg_fragmentation_in_percent FROM
sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'DETAILED') phystat inner JOIN sys.indexes i ON i.object_id = phystat.object_id AND i.index_id = phystat.index_id WHERE phystat.avg_fragmentation_in_percent > 10 AND phystat.avg_fragmentation_in_percent > 40



SELECT db_name(a.database_id) [Db Name]
,object_name(a.object_id) Table_Name
,a.index_id
,b.name
,a.avg_fragmentation_in_percent
,record_count,a.avg_fragment_size_in_pages,page_count,fragment_count FROM sys.dm_db_index_physical_stats (DB_ID(), null,NULL, NULL, 'DETAILED') AS a
JOIN sys.indexes AS b
ON a.object_id = b.object_id
AND a.index_id = b.index_id
where a.database_id = db_id()and
a.avg_fragmentation_in_percent>20/*and (OBJECT_NAME(b.object_id) like 'PX%' or OBJECT_NAME(b.object_id) like 'PM%')*/order by a.object_id

sp_updatestats