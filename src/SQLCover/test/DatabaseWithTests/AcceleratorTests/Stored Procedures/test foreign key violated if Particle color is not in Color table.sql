
CREATE PROCEDURE AcceleratorTests.[test foreign key violated if Particle color is not in Color table]
AS
BEGIN
  --Assemble: Fake the Particle and the Color tables to make sure they are empty and other 
  --          constraints will not be a problem
  EXEC tSQLt.FakeTable 'Accelerator.Particle';
  EXEC tSQLt.FakeTable 'Accelerator.Color';
  --          Put the FK_ParticleColor foreign key constraint back onto the Particle table
  --          so we can test it.
  EXEC tSQLt.ApplyConstraint 'Accelerator.Particle', 'FK_ParticleColor';
  
  --Act: Attempt to insert a record into the Particle table without any records in Color table.
  --     We expect an exception to happen, so we capture the ERROR_MESSAGE()
  DECLARE @err NVARCHAR(MAX); SET @err = '<No Exception Thrown!>';
  BEGIN TRY
    INSERT INTO Accelerator.Particle (ColorId) VALUES (7);
  END TRY
  BEGIN CATCH
    SET @err = ERROR_MESSAGE();
  END CATCH
  
  --Assert: Check that trying to insert the record resulted in the FK_ParticleColor foreign key being violated.
  --        If no exception happened the value of @err is still '<No Exception Thrown>'.
  IF (@err NOT LIKE '%FK_ParticleColor%')
  BEGIN
    EXEC tSQLt.Fail 'Expected exception (FK_ParticleColor exception) not thrown. Instead:',@err;
  END;
END;
