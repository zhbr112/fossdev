from string import ascii_letters
from tkinter.messagebox import showerror
import hashlib


class Person:
    """
    Представляет человека с такими атрибутами, как имя, электронная почта, паспортные данные и пароль.
    Включает методы для проверки этих атрибутов.
    """

    RUSSIAN_LETTERS = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя- "
    RUSSIAN_LETTERS_UPPER = RUSSIAN_LETTERS.upper()
    LETTERS = RUSSIAN_LETTERS + RUSSIAN_LETTERS_UPPER + ascii_letters
    DIGITS = "0123456789 "
    ALL_ALLOWED_SYMBOLS = LETTERS + DIGITS

    def __init__(self, fio, email, ps, password):
        """
        Инициализирует объект Person с заданными атрибутами.

        Args:
            fio (str): Полное имя человека.
            email (str): Адрес электронной почты человека.
            ps (str): Паспортные данные человека.
            password (str): Пароль человека.
        """
        self.fio = fio
        self.ps = ps
        self.email = email
        self.password = password

    @classmethod
    def validate_fio(cls, fio):
        """
        Проверяет полное имя (fio), чтобы убедиться, что оно содержит только буквы, дефисы и пробелы,
        и что оно состоит из трех частей (например, "Фамилия Имя Отчество").

        Args:
            fio (str): Полное имя для проверки.

        Raises:
            ValueError: Если полное имя содержит недопустимые символы или не соответствует правильному формату.
        """
        if not all(char in cls.LETTERS for char in fio.strip()):
            showerror(
                title="Предупреждение",
                message="В Ф.И.О. только буквенные символы, дефис и пробел",
            )
            raise ValueError("Недопустимые символы в ФИО")

        if len(fio.split()) != 3:
            showerror(
                title="Предупреждение", message="Ф.И.О. в формате(Иванов Иван Иванович)"
            )
            raise ValueError("ФИО должно содержать три части")

    @classmethod
    def validate_passport_details(cls, ps):
        """
        Проверяет паспортные данные (ps), чтобы убедиться, что они имеют формат "xxxx xxxxxx",
        где x - цифра.

        Args:
            ps (str): Паспортные данные для проверки.

        Raises:
            ValueError: Если паспортные данные не соответствуют правильному формату.
        """
        parts = ps.split()
        if (
            len(parts) != 2
            or len(parts[0]) != 4
            or len(parts[1]) != 6
            or not all(char in cls.DIGITS for char in ps.strip())
        ):
            showerror(
                title="Предупреждение",
                message="Паспорт данные в формате xxxx xxxxxx(x - цифра)",
            )
            raise ValueError("Неверный формат паспортных данных")

    @classmethod
    def validate_email(cls, email):
        """
        Проверяет адрес электронной почты, чтобы убедиться, что он содержит "@" и имеет части до и после "@".

        Args:
            email (str): Адрес электронной почты для проверки.

        Raises:
            ValueError: Если адрес электронной почты не соответствует правильному формату.
        """
        allowed_symbols = cls.ALL_ALLOWED_SYMBOLS + "@."
        parts = email.split("@")
        if (
            len(parts) != 2
            or not parts[0]
            or not parts[1]
            or not all(char in allowed_symbols for char in email.strip())
        ):
            showerror(
                title="Предупреждение",
                message="Электронная почта должна быть в формате ?*@*?(? - один сивол, * - любое колдичество сиволов)",
            )
            raise ValueError("Неверный формат электронной почты")

    @classmethod
    def validate_password(cls, password):
        """
        Проверяет пароль, чтобы убедиться, что он содержит только разрешенные символы и имеет ненулевую длину.

        Args:
            password (str): Пароль для проверки.

        Raises:
            ValueError: Если пароль содержит недопустимые символы или пустой.
        """
        if not password or not all(char in cls.ALL_ALLOWED_SYMBOLS for char in password):
            showerror(
                title="Предупреждение",
                message="Пароль может содержать только сиволы и длина должна быть больше 0",
            )
            raise ValueError("Неверный формат пароля")

    @property
    def fio(self):
        """
        Возвращает полное имя (fio).

        Returns:
            str: Полное имя.
        """
        return self.__fio

    @fio.setter
    def fio(self, f):
        """
        Устанавливает полное имя (fio) после его проверки.

        Args:
            f (str): Полное имя для установки.
        """
        self.validate_fio(f)
        self.__fio = f

    @property
    def ps(self):
        """
        Возвращает паспортные данные.

        Returns:
            str: Паспортные данные.
        """
        return self.__ps

    @ps.setter
    def ps(self, ps):
        """
        Устанавливает паспортные данные после их проверки.

        Args:
            ps (str): Паспортные данные для установки.
        """
        self.validate_passport_details(ps)
        self.__ps = ps

    @property
    def email(self):
        """
        Возвращает адрес электронной почты.

        Returns:
            str: Адрес электронной почты.
        """
        return self.__email

    @email.setter
    def email(self, email):
        """
        Устанавливает адрес электронной почты после его проверки.

        Args:
            email (str): Адрес электронной почты для установки.
        """
        self.validate_email(email)
        self.__email = email

    @property
    def password(self):
        """
        Возвращает пароль (в виде хеша).

        Returns:
            str: Хешированный пароль.
        """
        return self.__password

    @password.setter
    def password(self, password):
        """
        Устанавливает пароль после его проверки и хеширования.

        Args:
            password (str): Пароль для установки.
        """
        self.validate_password(password)
        self.__password = hashlib.sha256(password.encode()).hexdigest()
