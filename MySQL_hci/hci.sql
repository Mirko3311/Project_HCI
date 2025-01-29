-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `mydb` DEFAULT CHARACTER SET utf8 ;
USE `mydb` ;

-- -----------------------------------------------------
-- Table `mydb`.`Korisnik`
-- -----------------------------------------------------

CREATE TABLE IF NOT EXISTS `mydb`.`Korisnik` (
  `idKorisnik` INT  NOT NULL,
  `Ime` VARCHAR(45) NOT NULL,
  `Prezime` VARCHAR(45) NULL,
  `Email` VARCHAR(45) NULL,
  `Username` VARCHAR(45) NOT NULL,
  `Password` VARCHAR(255) NOT NULL,
  `Tip_korisnika` ENUM('student', 'profesor', 'superadministrator') NULL,
  PRIMARY KEY (`idKorisnik`),
  UNIQUE INDEX `Username_UNIQUE` (`Username` ASC) VISIBLE)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`Predmet`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`Predmet` (
  `idPredmeta` INT NOT NULL ,
  `Naziv` VARCHAR(45) NOT NULL,
  `Opis` VARCHAR(45) NULL,
  `ECTS` INT NULL,
  UNIQUE INDEX `idPredmet_UNIQUE` (`idPredmeta` ASC) VISIBLE,
  PRIMARY KEY (`idPredmeta`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`Student`

-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`Student` (
  `BrojIndeksa` VARCHAR(45) NOT NULL,
  `GodinaStudija` INT NOT NULL,
  `Korisnik_idKorisnik` INT NOT NULL,
  PRIMARY KEY (`Korisnik_idKorisnik`),
  UNIQUE INDEX `brojIndeksa_UNIQUE` (`BrojIndeksa` ASC) VISIBLE,
  INDEX `fk_Student_Korisnik1_idx` (`Korisnik_idKorisnik` ASC) VISIBLE,
  CONSTRAINT `fk_Student_Korisnik1`
    FOREIGN KEY (`Korisnik_idKorisnik`)
    REFERENCES `mydb`.`Korisnik` (`idKorisnik`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`Profesor`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`Profesor` (
  `Zvanje` VARCHAR(45) NULL,
  `Korisnik_idKorisnik` INT NOT NULL,
  INDEX `fk_Profesor_Korisnik1_idx` (`Korisnik_idKorisnik` ASC) VISIBLE,
  PRIMARY KEY (`Korisnik_idKorisnik`),
  CONSTRAINT `fk_Profesor_Korisnik1`
    FOREIGN KEY (`Korisnik_idKorisnik`)
    REFERENCES `mydb`.`Korisnik` (`idKorisnik`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`Predmet_Student`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`Predmet_Student` (
  `Predmet_idPredmeta` INT NOT NULL,
  `DatumUpisa` DATETIME NOT NULL,
  `Student_Korisnik_idKorisnik` INT NOT NULL,
  PRIMARY KEY (`Predmet_idPredmeta`, `Student_Korisnik_idKorisnik`),
  INDEX `fk_Predmet_has_Student_Predmet_idx` (`Predmet_idPredmeta` ASC) VISIBLE,
  INDEX `fk_Predmet_Student_Student1_idx` (`Student_Korisnik_idKorisnik` ASC) VISIBLE,
  CONSTRAINT `fk_Predmet_has_Student_Predmet`
    FOREIGN KEY (`Predmet_idPredmeta`)
    REFERENCES `mydb`.`Predmet` (`idPredmeta`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Predmet_Student_Student1`
    FOREIGN KEY (`Student_Korisnik_idKorisnik`)
    REFERENCES `mydb`.`Student` (`Korisnik_idKorisnik`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`DomaciZadatak`
-- -----------------------------------------------------
drop table `mydb`.`DomaciZadatak`;
CREATE TABLE IF NOT EXISTS `mydb`.`DomaciZadatak` (
  `idDomaciZadatak` VARCHAR(45) NOT NULL ,
  `Naziv` VARCHAR(45) NOT NULL,
  `Opis` VARCHAR(45) NULL,
  `Rok` DATETIME NOT NULL,
  `Predmet_idPredmeta` INT NOT NULL,
  `Profesor_Korisnik_idKorisnik` INT NOT NULL,
  PRIMARY KEY (`idDomaciZadatak`),
  INDEX `fk_DomaciZadatak_Predmet1_idx` (`Predmet_idPredmeta` ASC) VISIBLE,
  INDEX `fk_DomaciZadatak_Profesor1_idx` (`Profesor_Korisnik_idKorisnik` ASC) VISIBLE,
  CONSTRAINT `fk_DomaciZadatak_Predmet1`
    FOREIGN KEY (`Predmet_idPredmeta`)
    REFERENCES `mydb`.`Predmet` (`idPredmeta`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_DomaciZadatak_Profesor1`
    FOREIGN KEY (`Profesor_Korisnik_idKorisnik`)
    REFERENCES `mydb`.`Profesor` (`Korisnik_idKorisnik`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`Ispit`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`Ispit` (
  `DatumIspita` DATETIME NOT NULL,
  `Predmet_idPredmeta` INT NOT NULL,
  PRIMARY KEY (`Predmet_idPredmeta`, `DatumIspita`),
  INDEX `fk_Ispit_Predmet1_idx` (`Predmet_idPredmeta` ASC) VISIBLE,
  CONSTRAINT `fk_Ispit_Predmet1`
    FOREIGN KEY (`Predmet_idPredmeta`)
    REFERENCES `mydb`.`Predmet` (`idPredmeta`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`Ocjena`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`Ocjena` (
  `idOcjena` INT UNSIGNED NOT NULL,
  `Bodovi` VARCHAR(45) NULL,
  `Ocjena` DECIMAL NULL,
  `Datum` DATETIME NULL,
  `Predmet_idPredmeta` INT NOT NULL,
  `Profesor_Korisnik_idKorisnik` INT NOT NULL,
  `Student_Korisnik_idKorisnik` INT NOT NULL,
  PRIMARY KEY (`idOcjena`),
  INDEX `fk_Ocjena_Predmet1_idx` (`Predmet_idPredmeta` ASC) VISIBLE,
  INDEX `fk_Ocjena_Profesor1_idx` (`Profesor_Korisnik_idKorisnik` ASC) VISIBLE,
  INDEX `fk_Ocjena_Student1_idx` (`Student_Korisnik_idKorisnik` ASC) VISIBLE,
  CONSTRAINT `fk_Ocjena_Predmet1`
    FOREIGN KEY (`Predmet_idPredmeta`)
    REFERENCES `mydb`.`Predmet` (`idPredmeta`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Ocjena_Profesor1`
    FOREIGN KEY (`Profesor_Korisnik_idKorisnik`)
    REFERENCES `mydb`.`Profesor` (`Korisnik_idKorisnik`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Ocjena_Student1`
    FOREIGN KEY (`Student_Korisnik_idKorisnik`)
    REFERENCES `mydb`.`Student` (`Korisnik_idKorisnik`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`DomaciZadatak_Student`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`DomaciZadatak_Student` (
  `Bodovi` DECIMAL NULL,
  `DomaciZadatak_IdDomaciZadatak` INT NOT NULL,
  `Student_Korisnik_idKorisnik` INT NOT NULL,
  PRIMARY KEY (`DomaciZadatak_IdDomaciZadatak`, `Student_Korisnik_idKorisnik`),
  INDEX `fk_DomaciZadatak_Student_Student1_idx` (`Student_Korisnik_idKorisnik` ASC) VISIBLE,
  CONSTRAINT `fk_DomaciZadatak_has_Student_DomaciZadatak1`
    FOREIGN KEY (`DomaciZadatak_IdDomaciZadatak`)
    REFERENCES `mydb`.`DomaciZadatak` (`idDomaciZadatak`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_DomaciZadatak_Student_Student1`
    FOREIGN KEY (`Student_Korisnik_idKorisnik`)
    REFERENCES `mydb`.`Student` (`Korisnik_idKorisnik`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`Prisustvo`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`Prisustvo` (
  `Datum` DATETIME NOT NULL,
  `Status` ENUM('prisutan', 'odustan') NULL,
  `Predmet_idPredmeta` INT NOT NULL,
  `Student_Korisnik_idKorisnik` INT NOT NULL,
  PRIMARY KEY (`Datum`, `Predmet_idPredmeta`, `Student_Korisnik_idKorisnik`),
  INDEX `fk_Prisustvo_Predmet1_idx` (`Predmet_idPredmeta` ASC) VISIBLE,
  INDEX `fk_Prisustvo_Student1_idx` (`Student_Korisnik_idKorisnik` ASC) VISIBLE,
  CONSTRAINT `fk_Prisustvo_Predmet1`
    FOREIGN KEY (`Predmet_idPredmeta`)
    REFERENCES `mydb`.`Predmet` (`idPredmeta`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Prisustvo_Student1`
    FOREIGN KEY (`Student_Korisnik_idKorisnik`)
    REFERENCES `mydb`.`Student` (`Korisnik_idKorisnik`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
)
ENGINE = InnoDB;



-- -----------------------------------------------------
-- Table `mydb`.`Ispit_Student`
-- -----------------------------------------------------
drop table mydb.Ispit_Student;
CREATE TABLE IF NOT EXISTS mydb.Ispit_Student (
  Bodovi DECIMAL NULL,
  Ocjena INT,
  Ispit_DatumIspita DATETIME NOT NULL,
  Student_Korisnik_idKorisnik INT NOT NULL,
  Predmet_idPredmeta INT NOT NULL,
  PRIMARY KEY (Ispit_DatumIspita, Student_Korisnik_idKorisnik, Predmet_idPredmeta),
  INDEX fk_Ispit_Student_Student1_idx (Student_Korisnik_idKorisnik ASC) VISIBLE,
  INDEX fk_Ispit_Student_Predmet1_idx (Predmet_idPredmeta ASC) VISIBLE,
  CONSTRAINT fk_Ispit_Student_Student1
    FOREIGN KEY (Student_Korisnik_idKorisnik)
    REFERENCES mydb.Student (Korisnik_idKorisnik)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT fk_Ispit_Student_Predmet1
    FOREIGN KEY (Predmet_idPredmeta)
    REFERENCES mydb.Predmet (idPredmeta)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`Predmet_has_Profesor`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`Predmet_Profesor` (
  `Predmet_idPredmeta` INT NOT NULL,
  `Profesor_Korisnik_idKorisnik` INT NOT NULL,
  PRIMARY KEY (`Predmet_idPredmeta`, `Profesor_Korisnik_idKorisnik`),
  INDEX `fk_Predmet_has_Profesor_Profesor1_idx` (`Profesor_Korisnik_idKorisnik` ASC) VISIBLE,
  INDEX `fk_Predmet_has_Profesor_Predmet1_idx` (`Predmet_idPredmeta` ASC) VISIBLE,
  CONSTRAINT `fk_Predmet_has_Profesor_Predmet1`
    FOREIGN KEY (`Predmet_idPredmeta`)
    REFERENCES `mydb`.`Predmet` (`idPredmeta`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Predmet_has_Profesor_Profesor1`
    FOREIGN KEY (`Profesor_Korisnik_idKorisnik`)
    REFERENCES `mydb`.`Profesor` (`Korisnik_idKorisnik`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
