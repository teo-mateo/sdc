# sdc
schimb de carte
(book exchange)

#setup

create a database, name it whatever you like  
update the web.config SDCConnectionString to point to it.  

Open the solution in Visual Studio,  
build the solution,  
run Update-Database in the Package Manager console.  

run /etc/membership.sql to create the membership tables. 

if you want an admin account, create it with the app and then  
assign it the role named 'admin' by adding a new entry to the table 'webpages_UsersInRoles'
