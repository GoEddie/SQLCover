
$instance = "SQLCover"

Function Start-SqlDbInstance{
    Function Test-SqlDbInstance{
        ((sqllocaldb i |%{if ($_ -eq $instance){$_}} | Measure-Object).Count -eq 1)
    }
    
    Function New-SqlDbInstance{
        Write-Host "Creating sql local db instance"
        sqllocaldb c $instance
    }

    if(!(Test-SqlDbInstance)){
        New-SqlDbInstance
    }else{
        Write-Host "No need to create the sql local db instance"
    }

    Write-Host "Starting sql local db instance"
    sqllocaldb s $instance
}


Start-SqlDbInstance



