
CREATE PROCEDURE AcceleratorTests.[test email is sent if we detected a higgs-boson]
AS
BEGIN
  --Assemble: Replace the SendHiggsBosonDiscoveryEmail with a spy. 
  EXEC tSQLt.SpyProcedure 'Accelerator.SendHiggsBosonDiscoveryEmail';
  
  --Act: Call the AlertParticleDiscovered procedure - this is the procedure being tested.
  EXEC Accelerator.AlertParticleDiscovered 'Higgs Boson';
  
  --Assert: A spy records the parameters passed to the procedure in a *_SpyProcedureLog table. 
  --        Copy the EmailAddress parameter values that the spy recorded into the #Actual temp table.
  SELECT EmailAddress
    INTO #Actual
    FROM Accelerator.SendHiggsBosonDiscoveryEmail_SpyProcedureLog;
    
  --        Create an empty #Expected temp table that has the same structure as the #Actual table
  SELECT TOP(0) * INTO #Expected FROM #Actual;
  
  --        Add a row to the #Expected table with the expected email address.
  INSERT INTO #Expected 
    (EmailAddress)
  VALUES 
    ('particle-discovery@new-era-particles.tsqlt.org');

  --        Compare the data in the #Expected and #Actual tables
  EXEC tSQLt.AssertEqualsTable '#Expected', '#Actual';
END;
