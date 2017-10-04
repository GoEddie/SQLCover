
CREATE PROC AcceleratorTests.[test foreign key is not violated if Particle color is in Color table]
AS
BEGIN
  --Assemble: Fake the Particle and the Color tables to make sure they are empty and other 
  --          constraints will not be a problem
  EXEC tSQLt.FakeTable 'Accelerator.Particle';
  EXEC tSQLt.FakeTable 'Accelerator.Color';
  --          Put the FK_ParticleColor foreign key constraint back onto the Particle table
  --          so we can test it.
  EXEC tSQLt.ApplyConstraint 'Accelerator.Particle', 'FK_ParticleColor';
  
  --          Insert a record into the Color table. We'll reference this Id again in the Act
  --          step.
  INSERT INTO Accelerator.Color (Id) VALUES (7);
  
  --Act: Attempt to insert a record into the Particle table.
  INSERT INTO Accelerator.Particle (ColorId) VALUES (7);
  
  --Assert: If any exception was thrown, the test will automatically fail. Therefore, the test
  --        passes as long as there was no exception. This is one of the VERY rare cases when
  --        at test case does not have an Assert step.
END
