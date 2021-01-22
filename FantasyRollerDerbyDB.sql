USE [master]
GO

/****** Object:  Database [FantasyRollerDerby]    Script Date: 5/13/2017 3:07:01 PM ******/
CREATE DATABASE [FantasyRollerDerby]
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [FantasyRollerDerby].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [FantasyRollerDerby] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [FantasyRollerDerby] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [FantasyRollerDerby] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [FantasyRollerDerby] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [FantasyRollerDerby] SET ARITHABORT OFF 
GO

ALTER DATABASE [FantasyRollerDerby] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [FantasyRollerDerby] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [FantasyRollerDerby] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [FantasyRollerDerby] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [FantasyRollerDerby] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [FantasyRollerDerby] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [FantasyRollerDerby] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [FantasyRollerDerby] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [FantasyRollerDerby] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [FantasyRollerDerby] SET  ENABLE_BROKER 
GO

ALTER DATABASE [FantasyRollerDerby] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [FantasyRollerDerby] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [FantasyRollerDerby] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [FantasyRollerDerby] SET ALLOW_SNAPSHOT_ISOLATION ON 
GO

ALTER DATABASE [FantasyRollerDerby] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [FantasyRollerDerby] SET READ_COMMITTED_SNAPSHOT ON 
GO

ALTER DATABASE [FantasyRollerDerby] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [FantasyRollerDerby] SET RECOVERY FULL 
GO

ALTER DATABASE [FantasyRollerDerby] SET  MULTI_USER 
GO

ALTER DATABASE [FantasyRollerDerby] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [FantasyRollerDerby] SET DB_CHAINING OFF 
GO

ALTER DATABASE [FantasyRollerDerby] SET  READ_WRITE 
GO

