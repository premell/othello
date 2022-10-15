#!/bin/bash 
/usr/bin/mysqld_safe --skip-grant-tables &
mysql -u <fooUser> -e "CREATE DATABASE mydb" 
mysql -u <fooUser> mydb < schema.sql
