
CREATE PROCEDURE AcceleratorTests.[test a particle within the rectangle is returned]
AS
BEGIN
  --Assemble: Fake the Particle table to make sure it is empty and that constraints will not be a problem
  EXEC tSQLt.FakeTable 'Accelerator.Particle';
  --          Put a test particle into the table
  INSERT INTO Accelerator.Particle (Id, X, Y) VALUES (1, 0.5, 0.5);
  
  --Act: Call the  GetParticlesInRectangle Table-Valued Function and capture the Id column into the #Actual temp table
  SELECT Id
    INTO #Actual
    FROM Accelerator.GetParticlesInRectangle(0.0, 0.0, 1.0, 1.0);
  
  --Assert: Create an empty #Expected temp table that has the same structure as the #Actual table
  SELECT TOP(0) *
    INTO #Expected
    FROM #Actual;
  
  --        A single row with an Id value of 1 is expected
  INSERT INTO #Expected (Id) VALUES (1);

  --        Compare the data in the #Expected and #Actual tables
  EXEC tSQLt.AssertEqualsTable '#Expected', '#Actual';
END;
