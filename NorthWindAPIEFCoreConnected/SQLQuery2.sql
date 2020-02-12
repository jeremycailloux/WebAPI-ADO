declare @CategoryId as uniqueidentifier -- Category n'est pas auto-incrémenté, leur Id sont générés par Guid donc @....
set @CategoryId = newid()
insert Category (CategoryId, Name) 
values (@CategoryId, 'NewCategory')
insert Product (CategoryId, SupplierId, Name, UnitPrice, UnitsInStock)
values (@CategoryId, @SupplierId, @Name, @UnitPrice, @UnitsInStock)
