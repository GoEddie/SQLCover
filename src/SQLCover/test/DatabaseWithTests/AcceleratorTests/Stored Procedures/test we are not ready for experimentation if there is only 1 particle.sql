
CREATE PROCEDURE AcceleratorTests.[test we are not ready for experimentation if there is only 1 particle]
AS
BEGIN
  --Assemble: Fake the Particle table to make sure it is empty and has no constraints
  EXEC tSQLt.FakeTable 'Accelerator.Particle';
  INSERT INTO Accelerator.Particle (Id) VALUES (1);
  
  DECLARE @Ready BIT;
  
  --Act: Call the IsExperimentReady function
  SELECT @Ready = Accelerator.IsExperimentReady();
  
  --Assert: Check that 0 is returned from IsExperimentReady
  EXEC tSQLt.AssertEquals 0, @Ready;
  
END;
