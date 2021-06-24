create database Biblioteka_szkolna
GO
use Biblioteka_szkolna
GO
create table Wypożyczenia(
nr_wypożyczenia int not null IDENTITY CONSTRAINT pk_wypożyczenia PRIMARY KEY,
id_użytkownika int not null ,
id_książki int not null,
data_wyp date not null
)
GO

create table użytkownik(
nr_użytkownika int not null IDENTITY CONSTRAINT pk_użytkownik PRIMARY KEY,
imie varchar(20) not null,
nazwisko varchar(20) not null,
pesel  varchar (11) not null,
)

GO

create table Książka(
 id_książki int not null IDENTITY CONSTRAINT pk_Książka PRIMARY KEY,
 Tytuł varchar(20) not null,
 Autor varchar(20) not null,
 Dostępność varchar(3) not null
)

GO

insert into Książka values('Pan Tadeusz', 'Adam Mickiewicz','tak')
insert into Książka values('Pan Tadeusz', 'Adam Mickiewicz','tak')
insert into Książka values('Pan Tadeusz', 'Adam Mickiewicz','tak')
insert into Książka values('Pan Tadeusz', 'Adam Mickiewicz','tak')
insert into Książka values('Pan Tadeusz', 'Adam Mickiewicz','tak')
insert into Książka values('Lalka', 'Bolesław Prus','tak')
insert into Książka values('Lalka', 'Bolesław Prus','tak')
insert into Książka values('Lalka', 'Bolesław Prus','tak')
insert into Książka values('Lalka', 'Bolesław Prus','tak')
insert into Książka values('Lalka', 'Bolesław Prus','tak')

GO

create table pracownicy(
nr_pracownika int not null IDENTITY CONSTRAINT pk_pracownicy PRIMARY KEY,
imie varchar(20) not null,
nazwisko varchar(20) not null,
data_zatrudnienia date not null,
Pesel varchar(11) not null
)

GO


INSERT INTO [dbo].[pracownicy]
           ([imie]
           ,[nazwisko]
           ,[data_zatrudnienia]
           ,[Pesel])
     VALUES
           ('Dawid',
		   'Mielniczek',
		    '2012-10-24',
		   '12345678910')
		   
GO		 

create table Biblioteka(
 id_Opini int not null IDENTITY CONSTRAINT pk_Biblioteka PRIMARY KEY,
 id_użytkownika int not null ,
 Opinia varchar(150) not null,
)

GO  
