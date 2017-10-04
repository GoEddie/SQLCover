
CREATE PROCEDURE AcceleratorTests.[test status message includes the number of particles]
AS
BEGIN
  --Assemble: Fake the Particle table to make sure it is empty and that constraints will not be a problem
  EXEC tSQLt.FakeTable 'Accelerator.Particle';
  --          Put 3 test particles into the table
  INSERT INTO Accelerator.Particle (Id) VALUES (1);
  INSERT INTO Accelerator.Particle (Id) VALUES (2);
  INSERT INTO Accelerator.Particle (Id) VALUES (3);

  --Act: Call the GetStatusMessageFunction
  DECLARE @StatusMessage NVARCHAR(MAX);
  SELECT @StatusMessage = Accelerator.GetStatusMessage();

  --Assert: Make sure the status message is correct
  EXEC tSQLt.AssertEqualsString 'The Accelerator is prepared with 3 particles.', @StatusMessage;
END;
