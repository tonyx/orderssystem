# Orders System

  A tool for processing restaurant orders from the collection to the payment, emitting valid fiscal receipt.
  
  # news: 
  *I'm not maintaining this project anymore*, but rather I'm wodking on the portng on .net core 3.0: https://github.com/tonyx/orderssystem_core


## Getting Started

Typical use is restaurants collecting orders, printing orders where needed, and deliverying the fiscal receipts.


## main features:
1) managing ingredients, and categories of ingredients
2) managing courses, and categories
3) managing ingredients composing the courses (receipts)
4) collecting orders,
5) defining users roles,
6) managing variations of orders items (in term of add ons or drop off of ingredients)
7) managing price variations related to order items variations
8) printing orders by associating printer for course categories
9) displayng order items 
10) managing payment by eventually subdividing the bill
11) print a fiscal cash drawer (by using an external software).





These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites
  minimum:

  O.S.: Windows 7 or superior or Mac OS or Linux
  runtime/sdk: mono v 5.18  or .net framework 4.7

  postgres 9.6 or superior

### NEW: porting to .net core:
  https://github.com/tonyx/orderssystem_core
  


### Installing


1) clone the project:
```
git clone git@github.com:tonyx/orderssystem.git
```
2) in the subdirectory ordersystmprevious run the command:
```
nuget restore -SolutionDirectory .
```
3) install separately the Npgsql-2.2.1-net45, from https://www.nuget.org/packages/Npgsql/2.2.1. In case of troubles, you may want to
download the nupkg and then install from the local directory, by using the command 
```
nuget install Npgsql -version 2.2.1 -Source %cd% -OutputDirectory packages 
```
or
```
nuget install Npgsql -version 2.2.1 -Source `pwd` -OutputDirectory packages 
```
In th Db.fs file you may want to assign the resPath variable to the full path of the net45 subfolder of Npgsql.2.2.1 containing the Npgsql.dll (i.e. somepath/Npgsql.2.2.1/lib/net45) 
This will help resolving the db-entity binding at compile time, and will also help editing with ionide (providing you set fsautocomplete config to "net" rather than to "netcore")

4) change the App.config file if needed. If you specify https in App.config, then you need to provide a certificate
5) access to sql by psql command, and create a suave user:
```
create user suave with encrypted password '1234'
```
6) still in pgsql, create the database orderssystem
```
create database orderssystem
```
7) in pgsql, grant all priviledges to suave user
```
grant all privileges on database orderssystem to suave
```
9) load the orders_system.sql schema and data to orderssystem database, you may run the psql again form the command prompt:
```
psql -d ordersystem -f orders_system.sql
```
10) build the project by msbuild

11) connect locally using http://localhost:8083  using administrator/administror as login/password


## Connecting devices

You can use any printer to print order. A configuration tool in the program allow to associate a category of coureses (i.e. pizza, drink, etc...) to a specific division (pizzeria, bar, ...). The App.config contains the full path of a lpr print command. Tested on Mac os X. 
For fiscal receipt a special, file "ecr.txt" is generatd in a format compatible with a proprietary software WinEcrCom 1.9.20 from Ditron s.r.l. (which is distributed together with some fiscal devices).
Such software will look into the dir and print to a P.O.S. fiscal cash drawer when finding the ecr.txt file.
I succesfully tested it with a Sico Athom cash drawer.

I'll try to estend it by adopting  a standard non proprietary P.O.S. drivers/library.



## Built With

* [Suave.IO](https://suave.io/) - The web framework used
* [VSCode](https://code.visualstudio.com/) - source code editor
* [Ionide](http://ionide.io/) - Visual Studio Code extension for F#


## Authors

* **Antonio (Tonino) Lucca**  tonyx1@gmail.com

# Credits:
The code style is inspired by the musicstore example given with the Suave.IO. Basic functionalities like authentication is taken from their example.
The autocomplete feature in javascript is taken from https://www.w3schools.com/howto/howto_js_autocomplete.asp



## License

This project is licensed under the GPL License  (https://www.gnu.org/licenses/gpl.txt)


## Issues:

To print, a plain text file is sent to the printer. Special formatting (different sizes and stiles) will be one of the next features.

## Blog: 

https://tonyxzt.blogspot.com/2019/04/a-free-orders-system-in-f-part-1.html (part 1)


## Donations:

Donations are welcome.
I am also available for professional support services. Just drop me a message, in case.

[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=54LADGL7GWC9Y)
