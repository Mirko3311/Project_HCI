
SELECT k.ime, k.prezime, k.username, k.email, s.brojIndeksa, s.godinaStudija
FROM Student s
JOIN Korisnik k ON s.Korisnik_idKorisnik = k.idKorisnik
WHERE k.Tip_Korisnika= "student";

SELECT * from Korisnik k order by k.ime;
-- Unos u tabelu `Korisnik`
INSERT INTO `mydb`.`Korisnik` (`idKorisnik`,`Ime`, `Prezime`, `Email`, `Username`, `Password`, `Tip_korisnika`) VALUES
(1,'Jovan', 'Petrović', 'jovan.petrovic@email.com', 'jovan123', 'hashed_password_1', 'student'),
(2,'Marija', 'Ivić', 'marija.ivic@email.com', 'marija456', 'hashed_password_2', 'profesor'),
(3,'Aleksandar', 'Marković', 'aleksandar.markovic@email.com', 'alex789', 'hashed_password_3', 'superadministrator');
INSERT INTO Korisnik (Ime, Prezime, Email, Username, Password, Tip_korisnika)VALUES ("Marko", "Prezime", "@Email", "@Username", "@Password", "student");
-- Unos u tabelu `Predmet`
INSERT INTO `mydb`.`Predmet` (`idPredmeta`,`Naziv`, `Opis`, `ECTS`) VALUES
('2257','Programiranje u Javi', 'Osnove programiranja u Javi', 6),
('2222','Osnove baze podataka', 'Teorija i praksa baza podataka', 5),
('3412','Matematika 1', 'Osnovni matematički pojmovi i operacije', 7);

-- Unos u tabelu `Student`
INSERT INTO `mydb`.`Student` (`BrojIndeksa`, `GodinaStudija`, `Korisnik_idKorisnik`) VALUES
('S12345', 2, 1);

-- Unos u tabelu `Profesor`
INSERT INTO `mydb`.`Profesor` (`Zvanje`, `Korisnik_idKorisnik`) VALUES
('Docent', 2),
('Profesor', 3);

-- Unos u tabelu `Predmet_Student`
INSERT INTO `mydb`.`Predmet_Student` (`Predmet_idPredmeta`, `Student_Korisnik_idKorisnik`, `DatumUpisa`) VALUES
(1, 1, '2024-10-01 08:00:00'),
(2, 1, '2024-10-01 08:00:00');

-- Unos u tabelu `Profesor_Predmet`
INSERT INTO `mydb`.`Predmet_Profesor` (`Profesor_Korisnik_idKorisnik`, `Predmet_idPredmeta`) VALUES
(2, 1),
(3, 2);

-- Unos u tabelu `DomaciZadatak`
INSERT INTO `mydb`.`DomaciZadatak` (`Naziv`, `Opis`, `Rok`, `Predmet_idPredmeta`, `Profesor_Korisnik_idKorisnik`, `idDomaciZadatak`) VALUES
('Zadatak 1', 'Napisati program u Javi', '2024-11-01 23:59:59', 1, 2, 'DJAVA'),
('Zadatak 2', 'Prvi zadatak iz baze podataka', '2024-11-15 23:59:59', 2, 3,'PRVI');

-- Unos u tabelu `Ispit`
INSERT INTO `mydb`.`Ispit` (`DatumIspita`, `Predmet_idPredmeta`) VALUES
('2024-12-01 10:00:00', 1),
('2024-12-05 10:00:00', 2);



-- Unos u tabelu `DomaciZadatak_Student`
INSERT INTO `mydb`.`DomaciZadatak_Student` (`Bodovi`, `DomaciZadatak_IdDomaciZadatak`, `Student_Korisnik_idKorisnik`) VALUES
(8.5, 1, 1);


-- Unos u tabelu `Prisustvo`
INSERT INTO `mydb`.`Prisustvo` (`Datum`, `Status`, `Student_Korisnik_idKorisnik`, `Predmet_idPredmeta`) VALUES
('2024-10-01 08:00:00', 'prisutan', 1, 1),
('2024-10-02 08:00:00', 'odustan', 1, 2);

-- Unos u tabelu `Ispit_Student`
INSERT INTO `mydb`.`Ispit_Student` (`Student_Korisnik_idKorisnik`, `Bodovi`, `Predmet_idPredmeta`, `Ispit_DatumIspita`) VALUES
(1, 85, 1, '2024-12-01 10:00:00'),
(2, 75, 2, '2024-12-05 10:00:00');




SELECT 
    p.Naziv AS Predmet_Naziv, 
    p.Opis AS Predmet_Opis, 
    p.ECTS
 /*   k.Ime AS Profesor_Ime, 
    k.Prezime AS Profesor_Prezime, 
    k.Email AS Profesor_Email,
    k.Username AS Profesor_Username,
    pr.Zvanje AS Profesor_Zvanje*/
FROM 
    Predmet p
JOIN 
    Predmet_Profesor pp ON p.idPredmeta = pp.Predmet_idPredmeta
JOIN 
    Profesor pr ON pp.Profesor_Korisnik_idKorisnik = pr.Korisnik_idKorisnik
JOIN 
    Korisnik k ON pr.Korisnik_idKorisnik = k.idKorisnik
WHERE 
    pr.Korisnik_idKorisnik = '2';
    
ALTER TABLE Profesor
DROP FOREIGN KEY fk_Profesor_Korisnik1;

ALTER TABLE Profesor
ADD CONSTRAINT fk_Profesor_Korisnik1
FOREIGN KEY (Korisnik_idKorisnik) REFERENCES Korisnik(idKorisnik)
ON DELETE CASCADE;

    ALTER TABLE DomaciZadatak
DROP FOREIGN KEY fk_DomaciZadatak_Profesor1;

ALTER TABLE DomaciZadatak
ADD CONSTRAINT fk_DomaciZadatak_Profesor1
FOREIGN KEY (Profesor_Korisnik_idKorisnik) REFERENCES Profesor(Korisnik_idKorisnik)
ON DELETE CASCADE;



ALTER TABLE domacizadatak_student
DROP FOREIGN KEY fk_DomaciZadatak_has_Student_DomaciZadatak1;

ALTER TABLE domacizadatak_student
ADD CONSTRAINT fk_DomaciZadatak_has_Student_DomaciZadatak1
FOREIGN KEY (DomaciZadatak_IdDomaciZadatak)
REFERENCES DomaciZadatak(idDomaciZadatak)
ON DELETE CASCADE;

ALTER TABLE predmet_profesor
DROP FOREIGN KEY fk_Predmet_has_Profesor_Profesor1;

ALTER TABLE predmet_profesor
ADD CONSTRAINT fk_Predmet_has_Profesor_Profesor1
FOREIGN KEY (Profesor_Korisnik_idKorisnik) REFERENCES profesor(Korisnik_idKorisnik)
ON DELETE CASCADE;

ALTER TABLE student
DROP FOREIGN KEY fk_Student_Korisnik1;

ALTER TABLE student
ADD CONSTRAINT fk_Student_Korisnik1
FOREIGN KEY (Korisnik_idKorisnik) REFERENCES korisnik(idKorisnik)
ON DELETE CASCADE;



-- Isključivanje restrikcija za strane ključeve tokom brisanja
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;

-- Brisanje svih zapisa koji se odnose na profesora u povezanim tabelama
DELETE FROM `mydb`.`DomaciZadatak` 
WHERE `Profesor_Korisnik_idKorisnik` IN (SELECT `Korisnik_idKorisnik` FROM `mydb`.`Profesor` WHERE `Korisnik_idKorisnik` = 22);


DELETE FROM `mydb`.`Predmet_Profesor` 
WHERE `Profesor_Korisnik_idKorisnik` IN (SELECT `Korisnik_idKorisnik` FROM `mydb`.`Profesor` WHERE `Korisnik_idKorisnik` = 22);

-- Brisanje profesora iz tabele `Profesor`
DELETE FROM `mydb`.`Profesor` 
WHERE `Korisnik_idKorisnik` = 22;

-- Brisanje korisnika iz tabele `Korisnik`
DELETE FROM `mydb`.`Korisnik` 
WHERE `idKorisnik` = 22;

-- Vraćanje restrikcija za strane ključeve
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;

SELECT `Tip_korisnika`
FROM `mydb`.`Korisnik`
WHERE `Username` = 'jovan1234'
  AND `Password` = 'sifra1243';

SELECT k.idKorisnik, k.Ime, k.Prezime, k.Username, k.Email, k.Tip_korisnika, k.Password,p.Zvanje
            FROM korisnik k
            INNER JOIN profesor p ON p.Korisnik_idKorisnik = k.idKorisnik
            WHERE k.Username = "marija456";
select ime, prezime,email,username, password, s.BrojIndeksa, s.GodinaStudija
from Korisnik k
inner join Student s on k.idKorisnik = s. Korisnik_idKorisnik 
inner join Predmet_Student ps on ps.Student_Korisnik_idKorisnik = s.Korisnik_idKorisnik
where Predmet_idPredmeta = 2;