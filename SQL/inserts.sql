﻿USE XKOM
GO

INSERT INTO [User]([Name],LastName,[Password],Email)
VALUES
('Jakub','Cackowski','123','cyc'),
('Sasza','Ukraina','321','ukr');

INSERT INTO PaymentMethod(Name)
VALUES
('Card'),
('Card on delivery'),
('Cash on delivery'),
('Electronic bank transfer'),
('Blik');

INSERT INTO OrderStatus(Name)
VALUES
('Viewed'),
('In progress'),
('Completed'),
('Order stolen');

INSERT INTO ProductCategory(Name)
VALUES
('Laptops'),
('Computers'),
('Smartphones'),
('Graphic Cards'),
('Earphones'),
('Headphones'),
('Memory'),
('Storage'),
('Power supply & Chargers'),
('TV'),
('Consoles'),
('Motherboards'),
('Processors'),
('Computer cases'),
('RAM'),
('Cooling'),
('Keyboards'),
('Mouses'),
('Monitors'),
('Trigga');

INSERT INTO ProductCompany(Name)
VALUES 
('Apple'),
('Samsung'),
('Dell'),
('HP'),
('Logitech'),
('Lenovo'),
('Asus'),
('Acer'),
('Sony'),
('Intel'),
('AMD'),
('Corsair'),
('Razer'),
('HyperX'),
('Kingston'),
('Crucial'),
('SanDisk'),
('Cooler Master'),
('G.Skill'),
('Gigabyte'),
('LG Electronics'),
('Nvidia'),
('SteelSeries'),
('ADATA'),
('Xiaomi'),
('Jabra'),
('ENDORFY'),
('Africa');

INSERT INTO PromoCode(Code, StartDate, EndDate, Percentage, MaximumMoney)
VALUES 
('zakopane','2023-06-01','2023-06-03',100,999999.99),
('backuprider','2022-11-23','2023-11-23',25,1000),
('sql','2022-11-23','2023-11-23',35,550),
('cityfuse','2023-04-12','2023-07-03',0,10),
('rel','2022-10-23','2023-12-09',10,999.99);

GO

INSERT INTO Product(Name,Price,CategoryID,CompanyID,NumberAvailable,IntroductionDate,Properties)
VALUES 
('Xiaomi POCO M4 PRO',1099,3,25,223,'2022-03-16',
'{"Screen diagonal":"6,43","Processor":"MediaTek Helio G96","Graphic Card":"Mali-G57 MC2","RAM":"6GB","Storage":"128GB","Screen type":"Touchscreen, AMOLED","Refresh rate":"90Hz","Resolution":"2400 x 1080","Battery":"5000 mAh","Wireless charging": false}'
),
('SteelSeries Rival 650',369,18,23,321,'2019-05-19',
'{"Mouse type":"Gaming","Connection":"Wireless","Resolution":"12000dpi","Number of buttons":"7","Power supply":"Built-in battery"}'
),
('Samsung Odyssey G3',899,19,2,61,'2021-06-21',
'{"Screen diagonal":"24","Matrix coating":"Mat","Matrix type":"LED VA","Screen type":"Flat","Resolution":"1920x1080","Refresh rate":"144Hz","Reaction time":"1ms"}'
),
('Logitech G305 LIGHTSPEED',249,18,5,198,'2018-05-08',
'{"Mouse type":"Gaming","Connection":"Wireless","Resolution":"12000dpi","Number of buttons":"6","Power supply":"Battery AA"}'
),
('HP 15s',2499,1,4,458,'2019-12-07',
'{"Processor":"AMD Ryzen 5 5625U","Graphic card":"AMD Radeon Graphics","RAM":"16GB","Storage":"SSD M.2 PCIe 512GB","Screen diagonal":"15,6","Resolution":"1920x1080","Battery":"3454 mAh"}'
),
('Xiaomi POCO X5 Pro',1799,3,25,128,'2023-02-06',
'{"Screen diagonal":"6,67","Processor":"Qualcomm Snapdragon 778G","Graphic card":"Adreno 642L","RAM":"8GB","Storage":"256GB","Screen type":"Touchscreen, AMOLED","Refresh rate":"120Hz","Resolution":"2400 x 1080","Battery":"5000 mAh","Wireless charging": false}'
),
('Logitech G502 HERO K/DA',339,18,5,5,'2014-10-26',
'{"Mouse type":"Gaming","Connection":"Wire","Resolution":"25600dpi","Number of buttons":"11"}'
),
('Jabra Talk 25 SE',149,5,26,964,'2018-07-15',
'{"Connection":"Wireless","Using time":"9h","Standby time":"240h","Connectors":"micro USB"}'
),
('LG OLED55C21LA',5499,10,21,964,'2018-07-15',
'{"Screen diagonal":"55","Resolution":"UHD 4K 3840 x 2160","Refresh rate":"120Hz","TV type":"OLED","HDR":true}'
),
('ENDORFY Thock 75% Wireless',369,17,27,98,'2023-03-08',
'{"Switches":"Mechanical Kailh Box Red","Connection":"Wire, Wireless","Key illumination color":"Multicolor - RGB","Connectors":"USB-C x1"}'
),
('Samsung Portable SSD T7',599,8,2,11,'2023-01-17',
'{"Capacity":"2000TB","Interface":"USB 3.2 Gen. 2","Connectors":"USB Type-C","Read speed":"1050 MB/s","Write speed":"1000 MB/s"}'
),
('Tymek Rogowski',0,20,28,1,'2007-05-11',
'{"Color":"#000000","Speed":"0.(9)c","Specialization":"Basketball"}'
),
('Playstation 5',2699,11,9,0,'2020-11-12',
'{"Color":"Black&White","Processor":"AMD Ryzen Zen 2","Graphic card":"AMD RDNA 2.0","RAM":"16GB","Storage":"825 GB SSD"}'
),
('7000D Airflow',1449,14,12,0,'2019-07-04',
'{"Color":"Black","Type":"Full Tower","Side panel":"Tempered glass","RAM":"16GB","Mother board standard":"ATX","Number of fans":"3"}'
)

GO
USE master