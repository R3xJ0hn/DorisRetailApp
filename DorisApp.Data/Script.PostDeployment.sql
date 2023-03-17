IF NOT EXISTS (SELECT 1 FROM [dbo].Roles)
BEGIN
	INSERT INTO dbo.Roles(
		[RoleName],	
		[CreatedByUserId],	
		[UpdatedByUserId],	
		[CreatedAt],		
		[UpdatedAt]) 
	VALUES(
		'admin',				
		1,		
		1,		
		GETUTCDATE(),				
		GETUTCDATE())

	INSERT INTO dbo.Roles(
		[RoleName],	
		[CreatedByUserId],	
		[UpdatedByUserId],	
		[CreatedAt],		
		[UpdatedAt]) 
	VALUES(
		'cashier',				
		1,		
		1,		
		GETUTCDATE(),				
		GETUTCDATE())

	INSERT INTO dbo.Roles(
		[RoleName],	
		[CreatedByUserId],	
		[UpdatedByUserId],	
		[CreatedAt],		
		[UpdatedAt]) 
	VALUES(
		'manager',				
		1,		
		1,		
		GETUTCDATE(),				
		GETUTCDATE())
END

IF NOT EXISTS (SELECT 1 FROM [dbo].Users)
BEGIN
		INSERT INTO dbo.Users(
			[RoleID],			
			[FirstName],			
			[LastName],			
			[EmailAddress],		
			[PasswordHash],	
			[LastPasswordHash],	
			[LastPasswordCanged],
			[Token],
			[TokenCreated],
			[TokenExpires],
			[CreatedAt],			
			[UpdatedAt]) 
	VALUES(			
			1,				
			'super',			
			'admin',			
			'admin@gmail.com',		
			'$2a$11$tbmRRKfHwT9R56MTEY4FieGSHjSFMZJ6Mwos00MsCxRFe8LXPjHhe',		
			'$2a$11$tbmRRKfHwT9R56MTEY4FieGSHjSFMZJ6Mwos00MsCxRFe8LXPjHhe',
			GETUTCDATE(),
			'kX8XW8YBRnxKBypo20pN0EZiMMgiWrEcL9xNun/VUODQtPG2mPaxfUcGBW5QbEkZ/muFu5E0lm7sHJmpOKwyJA==.0',			
			GETUTCDATE(),		
			DATEADD(MONTH,1, GETUTCDATE()),	
			GETUTCDATE(),			
			GETUTCDATE())
END

IF NOT EXISTS (SELECT 1 FROM [dbo].Categories)
BEGIN
	INSERT INTO dbo.Categories(
		[CategoryName],	
		[CreatedByUserId],	
		[UpdatedByUserId],	
		[CreatedAt],		
		[UpdatedAt]) 
	VALUES(
		'undefine',				
		1,		
		1,		
		GETUTCDATE(),				
		GETUTCDATE())
END

IF NOT EXISTS (SELECT 1 FROM [dbo].SubCategories)
BEGIN
	INSERT INTO dbo.SubCategories(
		[SubCategoryName],
		[CategoryId],
		[CreatedByUserId],	
		[UpdatedByUserId],	
		[CreatedAt],		
		[UpdatedAt]) 
	VALUES(
		'undefine',
		@@IDENTITY,
		1,		
		1,		
		GETUTCDATE(),				
		GETUTCDATE())
END