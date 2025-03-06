from string import ascii_letters
from tkinter.messagebox import showerror
import hashlib


class Person:
    s_rus = "абвгдеёжзийклмнопрстуфхцчшщЪыьэюя- "
    s_rus_upper = s_rus.upper()
    letters = s_rus + s_rus_upper + ascii_letters
    digits = "0123456789 "
    symbols = letters + digits

    @classmethod
    def valid_fio(cls, fio):
        letters = cls.letters
        true_fio = True if len(fio.strip(letters)) == 0 else False
        if true_fio is False:
            showerror(
                title="Предупреждение",
                message="В Ф.И.О. только буквенные символы, дефис и пробел",
            )
            raise
        true_3 = True if len(fio.split()) == 3 else False
        if true_3 is False:
            showerror(
                title="Предупреждение", message="Ф.И.О. в формате(Иванов Иван Иванович)"
            )
            raise

    @classmethod
    def valid_ps(cls, ps):
        digits = cls.digits
        p = ps.split()
        true_ps = (
            True
            if (len(p) == 2)
            and (len(p[0]) == 4)
            and (len(p[1]) == 6)
            and (len(ps.strip(digits)) == 0)
            else False
        )
        if true_ps is False:
            showerror(
                title="Предупреждение",
                message="Паспорт данные в формате xxxx xxxxxx(x - цифра)",
            )
            raise

    @classmethod
    def valid_email(cls, ps):
        symbols = cls.symbols+'@.'
        p = ps.split('@')
        true_ps = (
            True
            if (len(p) == 2)
            and (len(p[0]) > 0)
            and (len(p[1]) > 0)
            and (len(ps.strip(symbols)) == 0)
            else False
        )
        if true_ps is False:
            showerror(
                title="Предупреждение",
                message="Электронная почта должна быть в формате ?*@*?(? - один сивол, * - любое колдичество сиволов)",
            )
            raise

    @classmethod
    def valid_pass(cls, ps):
        symbols = cls.symbols
        true_ps = (
            True
            if (len(ps.strip(symbols)) == 0)
            and (len(ps)>0)
            else False
        )
        if true_ps is False:
            showerror(
                title="Предупреждение",
                message="Пароль может содержать только сиволы и длина должна быть больше 0",
            )
            raise

    @property
    def fio(self):
        return self.__fio

    @fio.setter
    def fio(self, f):
        self.valid_fio(f)
        self.__fio = f

    @property
    def ps(self):
        return self.__ps

    @ps.setter
    def ps(self, ps):
        self.valid_ps(ps)
        self.__ps = ps

    @property
    def email(self):
        return self.__email

    @email.setter
    def email(self, email):
        self.valid_email(email)
        self.__email = email

    @property
    def password(self):
        return self.__password

    @password.setter
    def password(self, password):
        self.valid_pass(password)       
        self.__password = hashlib.sha256(password.encode()).hexdigest()

    def __init__(self, fio, email, ps, password):
        self.fio = fio
        self.ps = ps
        self.email = email
        self.password = password
