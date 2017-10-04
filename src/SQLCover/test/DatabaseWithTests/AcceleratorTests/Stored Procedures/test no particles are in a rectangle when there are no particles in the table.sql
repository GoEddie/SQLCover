
CREATE PROCEDURE AcceleratorTests.[test no particles are in a rectangle when there are no particles in the table]
AS
BEGIN
  --Assemble: Fake the Particle table to make sure it is empty
  EXEC tSQLt.FakeTable 'Accelerator.Particle';

  DECLARE @ParticlesInRectangle INT;
  
  --Act: Call the  GetParticlesInRectangle Table-Valued Function and capture the number of rows it returns.
  SELECT @ParticlesInRectangle = COUNT(1)
    FROM Accelerator.GetParticlesInRectangle(0.0, 0.0, 1.0, 1.0);
  
  --Assert: Check that 0 rows were returned
  EXEC tSQLt.AssertEquals 0, @ParticlesInRectangle;
END;
