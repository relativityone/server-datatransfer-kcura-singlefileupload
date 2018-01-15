IF EXISTS(SELECT TOP 1 1 FROM EDDSDBO.Settings WHERE [Name] = 'SFUFieldValues')
BEGIN
	UPDATE 
		EDDSDBO.Settings 
	SET 
		[Value] = '{0}'
	WHERE
		[Name] = 'SFUFieldValues'

END
ELSE
BEGIN
	INSERT INTO
		EDDSDBO.Settings
		(
		[Name],
		[Value]
		)
		VALUES(
		'SFUFieldValues',
		'{0}'
	)
END	