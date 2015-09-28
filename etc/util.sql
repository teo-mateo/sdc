SELECT OBJECT_NAME(i.object_id) AS TableName ,i.name AS TableIndexName ,
phystat.avg_fragmentation_in_percent FROM
sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'DETAILED') phystat inner JOIN sys.indexes i ON i.object_id = phystat.object_id AND i.index_id = phystat.index_id WHERE phystat.avg_fragmentation_in_percent > 10 AND phystat.avg_fragmentation_in_percent > 40


--warning: this loads the entire db to the memory cache
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

-- find what is in the sql server memory cache
select
       count(*)as cached_pages_count,
       obj.name as objectname,
       ind.name as indexname,
       obj.index_id as indexid
from sys.dm_os_buffer_descriptors as bd
    inner join
    (
        select       object_id as objectid,
                           object_name(object_id) as name,
                           index_id,allocation_unit_id
        from sys.allocation_units as au
            inner join sys.partitions as p
                on au.container_id = p.hobt_id
                    and (au.type = 1 or au.type = 3)
        union all
        select       object_id as objectid,
                           object_name(object_id) as name,
                           index_id,allocation_unit_id
        from sys.allocation_units as au
            inner join sys.partitions as p
                on au.container_id = p.partition_id
                    and au.type = 2
    ) as obj
        on bd.allocation_unit_id = obj.allocation_unit_id
left outer join sys.indexes ind 
  on  obj.objectid = ind.object_id
 and  obj.index_id = ind.index_id
where bd.database_id = db_id()
  and bd.page_type in ('data_page', 'index_page')
group by obj.name, ind.name, obj.index_id
order by cached_pages_count desc


--deadlock info
SELECT  L.request_session_id AS SPID, 
    DB_NAME(L.resource_database_id) AS DatabaseName,
    O.Name AS LockedObjectName, 
    P.object_id AS LockedObjectId, 
    L.resource_type AS LockedResource, 
    L.request_mode AS LockType,
    ST.text AS SqlStatementText,        
    ES.login_name AS LoginName,
    ES.host_name AS HostName,
    TST.is_user_transaction as IsUserTransaction,
    AT.name as TransactionName,
    CN.auth_scheme as AuthenticationMethod
FROM    sys.dm_tran_locks L
    JOIN sys.partitions P ON P.hobt_id = L.resource_associated_entity_id
    JOIN sys.objects O ON O.object_id = P.object_id
    JOIN sys.dm_exec_sessions ES ON ES.session_id = L.request_session_id
    JOIN sys.dm_tran_session_transactions TST ON ES.session_id = TST.session_id
    JOIN sys.dm_tran_active_transactions AT ON TST.transaction_id = AT.transaction_id
    JOIN sys.dm_exec_connections CN ON CN.session_id = ES.session_id
    CROSS APPLY sys.dm_exec_sql_text(CN.most_recent_sql_handle) AS ST
WHERE   resource_database_id = db_id()
ORDER BY L.request_session_id
