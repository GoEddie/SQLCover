
CREATE PROCEDURE 
  AcceleratorTests.[test ready for experimentation if 2 particles]
AS
BEGIN
  --Assemble: Fake the Particle table to make sure 
  --          it is empty and has no constraints
  EXEC tSQLt.FakeTable 'Accelerator.Particle';
  INSERT INTO Accelerator.Particle (Id) VALUES (1);
  INSERT INTO Accelerator.Particle (Id) VALUES (2);
  
  DECLARE @Ready BIT;
  
  --Act: Call the IsExperimentReady function
  SELECT @Ready = Accelerator.IsExperimentReady();
  
  --Assert: Check that 1 is returned from IsExperimentReady
  EXEC tSQLt.AssertEquals 1, @Ready;
  
END;
