from tkinter import ttk
from tkinter import *
import tkinter as tk
import sqlite3
from tkinter import ttk, messagebox
from tkinter.messagebox import showinfo, showerror
from Person import Person
import hashlib
from email.mime.multipart import MIMEMultipart
from email.mime.text import MIMEText
import smtplib


gl_user = ''

def window_pass():
    win1 = Tk()
    win1.title("Введите пароль")
    win1.geometry("250x120")

    frame = ttk.Frame(win1, borderwidth=1, relief=SOLID, padding=[8, 10])
    label = ttk.Label(frame, text='Пароль')
    label.pack(anchor=NW)
    entry_pass = ttk.Entry(frame)
    entry_pass.pack(anchor=NW)
    frame.pack(anchor=NW, fill=X, padx=5, pady=5)

    def sigin_in():
        global gl_user
        user = tree.item(tree.selection()[0])["values"][2]
        cur.execute('SELECT password FROM Users WHERE email = ?', (user,))
        password = cur.fetchall()
        if password[0][0] == hashlib.sha256(entry_pass.get().encode()).hexdigest():
            notebook.select(1)        
            for i in range(notebook1.index("end")):
                notebook1.forget(0)
            gl_user=user
            load_notes()
        else:
            showerror(
                title="Предупреждение",
                message="Пароль неверный",
            )
            raise

        win1.destroy()

    tk.Button(win1, text='ОК', command=sigin_in).place(rely=1, width=50, height=30, relx=0.5, anchor=tk.S, y=-20)
    win1.mainloop()
    

def window():
    win1 = Tk()
    win1.title("Добавление")
    win1.geometry("250x350")

    frame = ttk.Frame(win1, borderwidth=1, relief=SOLID, padding=[8, 10])
    label = ttk.Label(frame, text='ФИО')
    label.pack(anchor=NW)
    entry_fio = ttk.Entry(frame)
    entry_fio.pack(anchor=NW)
    frame.pack(anchor=NW, fill=X, padx=5, pady=5)

    frame = ttk.Frame(win1, borderwidth=1, relief=SOLID, padding=[8, 10])
    label = ttk.Label(frame, text='Пасп. данные')
    label.pack(anchor=NW)
    entry_ps = ttk.Entry(frame)
    entry_ps.pack(anchor=NW)
    frame.pack(anchor=NW, fill=X, padx=5, pady=5)

    frame = ttk.Frame(win1, borderwidth=1, relief=SOLID, padding=[8, 10])
    label = ttk.Label(frame, text='email')
    label.pack(anchor=NW)
    entry_email = ttk.Entry(frame)
    entry_email.pack(anchor=NW)
    frame.pack(anchor=NW, fill=X, padx=5, pady=5)

    frame = ttk.Frame(win1, borderwidth=1, relief=SOLID, padding=[8, 10])
    label = ttk.Label(frame, text='Пароль')
    label.pack(anchor=NW)
    entry_pass = ttk.Entry(frame)
    entry_pass.pack(anchor=NW)
    frame.pack(anchor=NW, fill=X, padx=5, pady=5)

    def add_user():
        pers = Person(entry_fio.get(), entry_email.get(), entry_ps.get(), entry_pass.get())
        cur.execute("INSERT INTO Users (fio, email, ps, password) VALUES (?, ?, ?, ?)",
                    (pers.fio, pers.email, pers.ps, pers.password))
        cur.execute('SELECT * FROM Users')
        users = cur.fetchall()
        tree.delete(*tree.get_children())
        for user in users:
            tree.insert("", END, values=(user[0], user[2], user[1]))
        win1.destroy()

    tk.Button(win1, text='ОК', command=add_user).place(rely=1, width=50, height=30, relx=0.5, anchor=tk.S, y=-20)
    win1.mainloop()

# Формирование функции дополнительного окна регистрации
def window1():
    win1 = Tk()
    win1.title("Измените данные")
    win1.geometry("250x350")

    frame = ttk.Frame(win1, borderwidth=1, relief=SOLID, padding=[8, 10])
    label = ttk.Label(frame, text='ФИО')
    label.pack(anchor=NW)
    entry_fio = ttk.Entry(frame)
    entry_fio.pack(anchor=NW)
    frame.pack(anchor=NW, fill=X, padx=5, pady=5)

    frame = ttk.Frame(win1, borderwidth=1, relief=SOLID, padding=[8, 10])
    label = ttk.Label(frame, text='Пасп. данные')
    label.pack(anchor=NW)
    entry_ps = ttk.Entry(frame)
    entry_ps.pack(anchor=NW)
    frame.pack(anchor=NW, fill=X, padx=5, pady=5)

    frame = ttk.Frame(win1, borderwidth=1, relief=SOLID, padding=[8, 10])
    label = ttk.Label(frame, text='email')
    label.pack(anchor=NW)
    entry_email = ttk.Entry(frame)
    entry_email.pack(anchor=NW)
    frame.pack(anchor=NW, fill=X, padx=5, pady=5)

    frame = ttk.Frame(win1, borderwidth=1, relief=SOLID, padding=[8, 10])
    label = ttk.Label(frame, text='Пароль')
    label.pack(anchor=NW)
    entry_pass = ttk.Entry(frame)
    entry_pass.pack(anchor=NW)
    frame.pack(anchor=NW, fill=X, padx=5, pady=5)

    def update_user():
        cur.execute('DELETE FROM Users WHERE email = ?', (tree.item(tree.selection()[0])["values"][2],))
        tree.delete(tree.selection()[0])
        pers = Person(entry_fio.get(), entry_email.get(), entry_ps.get(), entry_pass.get())
        cur.execute("INSERT INTO Users (fio, email, ps, password) VALUES (?, ?, ?, ?)",
                    (pers.fio, pers.email, pers.ps, pers.password))
        cur.execute('SELECT * FROM Users')
        users = cur.fetchall()
        tree.delete(*tree.get_children())
        for user in users:
            tree.insert("", END, values=(user[0], user[2], user[1]))
        win1.destroy()

    tk.Button(win1, text='ОК', command=update_user).place(rely=1, width=50, height=30, relx=0.5, anchor=tk.S, y=-20)
    win1.mainloop()


def del_user():
    for i in tree.selection():
        cur.execute('DELETE FROM Users WHERE email = ?', (tree.item(i)["values"][2],))
        tree.delete(i)


db = sqlite3.connect("DB.db")
cur = db.cursor()

cur.execute("""CREATE TABLE IF NOT EXISTS Users (
    fio    TEXT,
    email TEXT PRIMARY KEY,
    ps TEXT,   
    password Text  
);
""")
cur.execute("""CREATE TABLE IF NOT EXISTS Notes (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    email TEXT,
    title TEXT,   
    content Text  
);
""")
db.commit()

win = tk.Tk()
win.title('Заметки')
win.geometry('800x500')
style1 = ttk.Style()
style1.configure("Myw.TLabel", background="#4682B4")

# Формирование вкладок
notebook = ttk.Notebook()
notebook.pack(expand=True, fill=tk.BOTH)
frame1 = ttk.Frame(notebook, style="Myw.TLabel")  # свое название.стандартный стиль
frame2 = ttk.Frame(notebook, style="Myw.TLabel")

frame1.pack(fill=tk.BOTH, expand=True)
frame2.pack(fill=tk.BOTH, expand=True)
notebook.add(frame1, text="Авторизация")
notebook.add(frame2, text="Ежедневник")

notebook.hide(1)


# Формирование таблицы
label = ttk.Label(frame1, text="Добро пожаловать в заметки!", background='#4682B4', foreground='#FFFFFF',font=("Arial", 18))
label.pack(side=tk.TOP)

person = []
columns = ("Fname", "Ps", "Email")

tree = ttk.Treeview(frame1, columns=columns, show="headings")
tree.place(relx=0.02, rely=0.45, anchor='w')

tree.heading("Fname", text="ФИО")
tree.heading("Ps", text="Пасп.данные")
tree.heading("Email", text="Почта")

tree.column("#1", stretch=NO, width=220)
tree.column("#2", stretch=NO, width=180)
tree.column("#3", stretch=NO, width=180)

cur.execute('SELECT * FROM Users')
users = cur.fetchall()
for user in users:
    tree.insert("", END, values=(user[0], user[2], user[1]))

# Формирование кнопок
position1 = {"width": 100, "height": 40, 'rely': 0, 'relx': 1, 'anchor': NE}
list_el = ["Добавить", "Удалить", "Изменить"]
list_fn = [window, del_user, window1]
for i in range(3):
    tk.Button(frame1, text=list_el[i], command=list_fn[i]).place(y=60 * i + 120, x=-13, **position1)
list_fn = [window, window, window]
position1 = {"width": 100, "height": 40, 'rely': 1, 'relx': 0, 'anchor': SE}
list_el1 = ["Войти", "Выйти"]
list_fn1 = [window_pass, win.destroy]
for i in range(2):
    tk.Button(frame1, text=list_el1[i], command=list_fn1[i]).place(x=120 * i + 663, y=-3, **position1)

# Формирование второго фрейма

notes = {}
notebook1 = ttk.Notebook(frame2)
notebook1.pack(padx=20, pady=10, fill=tk.BOTH, expand=True)


def add_notes():
    note_frame = ttk.Frame(notebook1)
    notebook1.add(note_frame, text="Новая заметка")

    title_label = ttk.Label(note_frame, text="Тема:")
    title_label.grid(row=0, column=0, padx=10, pady=10, sticky="W")

    title_entry = ttk.Entry(note_frame, width=53)
    title_entry.grid(row=0, column=1, padx=10, pady=10)

    content_label = ttk.Label(note_frame, text="Описание:")
    content_label.grid(row=1, column=0, padx=10, pady=10, sticky="W")

    content_entry = tk.Text(note_frame, width=40, height=10)
    content_entry.grid(row=1, column=1, padx=10, pady=10)

    def save_notes():
        title = title_entry.get()
        content = content_entry.get("1.0", tk.END)
        notes[title] = content.strip()

        cur.execute("INSERT INTO Notes (email, title, content) VALUES (?, ?, ?)",
                    (gl_user, title, content))

        note_content = tk.Text(notebook1, width=40, height=10)
        note_content.insert(tk.END, content)
        notebook1.forget(notebook1.select())
        notebook1.add(note_content, text=title)

    tk.Button(note_frame, text="ОК", command=save_notes, background="#4682B4",foreground="#FFFFFF").place(**position1, x=550, y=-284)


def delete_notes():
    current_tab = notebook1.index(notebook1.select())
    note_title = notebook1.tab(current_tab, "text")
    confirm = messagebox.askyesno('Предупреждение', f"Вы действительно хотите удалить {note_title}?")
    print(confirm)
    if confirm:
        notebook1.forget(current_tab)
        cur.execute('DELETE FROM Notes WHERE email = ? AND title = ?', (gl_user, note_title))


def load_notes():
    try:
        cur.execute('SELECT title, content FROM Notes WHERE email = ?', (gl_user,))
        note = cur.fetchall()
        for title, content in note:
            note_content = tk.Text(notebook1, width=40, height=10)
            note_content.insert(tk.END, content)
            notebook1.add(note_content, text=title)
    except Exception as e:
        showerror(title='предупреждение', message=f'Error => {e}')



list_el2 = ["Новая заметка", "Удалить заметку"]
list_fn2 = [add_notes, delete_notes]
for i in range(2):
    tk.Button(frame2, text=list_el2[i], command=list_fn2[i], background="#4682B4",foreground="#FFFFFF").place(x=120 * i + 650, y=-25, **position1)

win.mainloop()
db.commit()
db.close()
