select newid();
insert Address (AddressId, Street, City, PostalCode, Country)
values ('598E0BAE-EBA3-4A55-ADC2-4A0142CFE151', 'Rue GTM', 'Paris', '75014', 'France')
insert Supplier (AddressId, CompanyName) 
values ('598E0BAE-EBA3-4A55-ADC2-4A0142CFE151', 'OneSupplier')

insert Address (AddressId, Street, City, PostalCode, Country)
values ('0903CCD1-E872-41A3-8815-9DEFE6A15D09', 'Rue GTM', 'Paris', '75014', 'France')
insert Supplier (AddressId, CompanyName) 
values ('0903CCD1-E872-41A3-8815-9DEFE6A15D09', 'OneSupplier')