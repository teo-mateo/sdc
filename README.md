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

Change this line in web.config, remove the tempDirectory attribute if it is present.
    <compilation debug="true" targetFramework="4.5.2" batch="false" tempDirectory="F:\TEMP\iistemp" />

if you want an admin account, create it with the app and then  
assign it the role named 'admin' by changing its entry in the table 'webpages_UsersInRoles', role=1

ROLES: 1=admin, 2=curator, 3=user.

Development streamed live at https://www.livecoding.tv/tbardici/
