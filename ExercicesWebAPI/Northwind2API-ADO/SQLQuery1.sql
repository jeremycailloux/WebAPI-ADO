select distinct A.Country from Supplier S
inner join Address A on(S.AddressId  = A.AddressId)
/* left outer join on obtiendrait les fournisseurs qui n ont pas d adresse */

select S.SupplierId, S.CompanyName from Supplier S
inner join Address A on(A.AddressId = S.AddressId)
where A.Country = 'Australia'

select count(P.ProductId) from Product P
inner join Supplier S on(P.SupplierId = S.SupplierId)
where S.SupplierId = @supplierid

/* créer et utiliser une fonction sql */
drop function if exists ufn_Country
go
create function ufn_Country (@country nvarchar(40))
returns int
begin
    return (select count (*) from Product P
    inner join Supplier SU on(P.SupplierId = SU.SupplierId)
    inner join Address A on(SU.AddressId  = A.AddressId)
    where A.country = @country)
end
go

select dbo.ufn_Country('France')
go

select * from Product
where productId = 1
go

insert Product(CategoryId, SupplierId, Name, UnitPrice, UnitsInStock)
values (@CategoryId, @SupplierId, @Name, @UnitPrice, @UnitsInStock)
SELECT cast(IDENT_CURRENT('Product') as int)

update Product set CategoryId = @CategoryId, SupplierId = @SupplierId, Name = @Name, UnitPrice = @UnitPrice, UnitsInStock = @UnitsInStock where ProductId = @ProductId

delete from Product where ProductId = @productid


if not exists (select Name from Category where Name = '00000000-0000-0000-0000-000000000000')
insert Category (CategoryId, Name)
values (@CategoryId, 'Others')
