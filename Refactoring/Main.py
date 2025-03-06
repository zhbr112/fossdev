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

class NoteApp:
    def __init__(self, master):
        self.master = master
        master.title('Заметки')
        master.geometry('800x500')

        self.db = sqlite3.connect("DB.db")
        self.cur = self.db.cursor()
        self.gl_user = ''
        self.notes = {}

        self.create_tables()
        self.create_gui()
        self.master.mainloop()

    def create_tables(self):
        self.cur.execute("""CREATE TABLE IF NOT EXISTS Users (
            fio    TEXT,
            email TEXT PRIMARY KEY,
            ps TEXT,   
            password Text  
        );
        """)
        self.cur.execute("""CREATE TABLE IF NOT EXISTS Notes (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            email TEXT,
            title TEXT,   
            content Text  
        );
        """)
        self.db.commit()

    def create_gui(self):
        style1 = ttk.Style()
        style1.configure("Myw.TLabel", background="#4682B4")

        # Form tabs
        self.notebook = ttk.Notebook(self.master)
        self.notebook.pack(expand=True, fill=tk.BOTH)
        self.frame1 = ttk.Frame(self.notebook, style="Myw.TLabel")
        self.frame2 = ttk.Frame(self.notebook, style="Myw.TLabel")

        self.frame1.pack(fill=tk.BOTH, expand=True)
        self.frame2.pack(fill=tk.BOTH, expand=True)
        self.notebook.add(self.frame1, text="Авторизация")
        self.notebook.add(self.frame2, text="Ежедневник")

        self.notebook.hide(1)

        self.create_auth_frame()
        self.create_notes_frame()

    def create_auth_frame(self):
        # Authorization Frame
        label = ttk.Label(self.frame1, text="Добро пожаловать в заметки!", background='#4682B4', foreground='#FFFFFF', font=("Arial", 18))
        label.pack(side=tk.TOP)

        columns = ("Fname", "Ps", "Email")
        self.tree = ttk.Treeview(self.frame1, columns=columns, show="headings")
        self.tree.place(relx=0.02, rely=0.45, anchor='w')

        self.tree.heading("Fname", text="ФИО")
        self.tree.heading("Ps", text="Пасп.данные")
        self.tree.heading("Email", text="Почта")

        self.tree.column("#1", stretch=NO, width=220)
        self.tree.column("#2", stretch=NO, width=180)
        self.tree.column("#3", stretch=NO, width=180)

        self.load_users()

        position1 = {"width": 100, "height": 40, 'rely': 0, 'relx': 1, 'anchor': NE}
        list_el = ["Добавить", "Удалить", "Изменить"]
        list_fn = [self.window_add_user, self.del_user, self.window_update_user]
        for i in range(3):
            tk.Button(self.frame1, text=list_el[i], command=list_fn[i]).place(y=60 * i + 120, x=-13, **position1)

        position2 = {"width": 100, "height": 40, 'rely': 1, 'relx': 0, 'anchor': SE}
        list_el1 = ["Войти", "Выйти"]
        list_fn1 = [self.window_pass, self.master.destroy]
        for i in range(2):
            tk.Button(self.frame1, text=list_el1[i], command=list_fn1[i]).place(x=120 * i + 663, y=-3, **position2)

    def create_notes_frame(self):
        # Notes Frame
        self.notebook1 = ttk.Notebook(self.frame2)
        self.notebook1.pack(padx=20, pady=10, fill=tk.BOTH, expand=True)

        position3 = {"width": 120, "height": 40, 'rely': 1, 'relx': 0, 'anchor': SE}
        list_el2 = ["Новая заметка", "Удалить заметку"]
        list_fn2 = [self.add_notes, self.delete_notes]
        for i in range(len(list_el2)):
            tk.Button(self.frame2, text=list_el2[i], command=list_fn2[i], background="#4682B4", foreground="#FFFFFF").place(x=140 * i + 620, y=-25, **position3)
    
    def load_users(self):
        self.cur.execute('SELECT * FROM Users')
        users = self.cur.fetchall()
        for user in users:
            self.tree.insert("", END, values=(user[0], user[2], user[1]))


    def window_pass(self):
        win1 = tk.Toplevel(self.master)
        win1.title("Введите пароль")
        win1.geometry("250x120")

        frame = ttk.Frame(win1, borderwidth=1, relief=tk.SOLID, padding=[8, 10])
        label = ttk.Label(frame, text='Пароль')
        label.pack(anchor=tk.NW)
        entry_pass = ttk.Entry(frame, show="*") #hide password
        entry_pass.pack(anchor=tk.NW)
        frame.pack(anchor=tk.NW, fill=tk.X, padx=5, pady=5)

        def sigin_in():
            try:
                user = self.tree.item(self.tree.selection()[0])["values"][2]
                self.cur.execute('SELECT password FROM Users WHERE email = ?', (user,))
                password = self.cur.fetchall()
                if password[0][0] == hashlib.sha256(entry_pass.get().encode()).hexdigest():
                    self.notebook.select(1)
                    for i in range(self.notebook1.index("end")):
                        self.notebook1.forget(0)
                    self.gl_user = user
                    self.load_notes()
                    win1.destroy()
                else:
                    showerror(
                        title="Предупреждение",
                        message="Пароль неверный",
                    )
            except IndexError:
                showerror(title="Error", message="Please select user")
                win1.destroy()
                return
                
        tk.Button(win1, text='ОК', command=sigin_in).place(rely=1, width=50, height=30, relx=0.5, anchor=tk.S, y=-20)

    def window_add_user(self):
        win1 = tk.Toplevel(self.master)
        win1.title("Добавление")
        win1.geometry("250x350")

        # ... (Code for user input frames remains the same)
        frame = ttk.Frame(win1, borderwidth=1, relief=tk.SOLID, padding=[8, 10])
        label = ttk.Label(frame, text='ФИО')
        label.pack(anchor=tk.NW)
        entry_fio = ttk.Entry(frame)
        entry_fio.pack(anchor=tk.NW)
        frame.pack(anchor=tk.NW, fill=tk.X, padx=5, pady=5)

        frame = ttk.Frame(win1, borderwidth=1, relief=tk.SOLID, padding=[8, 10])
        label = ttk.Label(frame, text='Пасп. данные')
        label.pack(anchor=tk.NW)
        entry_ps = ttk.Entry(frame)
        entry_ps.pack(anchor=tk.NW)
        frame.pack(anchor=tk.NW, fill=tk.X, padx=5, pady=5)

        frame = ttk.Frame(win1, borderwidth=1, relief=tk.SOLID, padding=[8, 10])
        label = ttk.Label(frame, text='email')
        label.pack(anchor=tk.NW)
        entry_email = ttk.Entry(frame)
        entry_email.pack(anchor=tk.NW)
        frame.pack(anchor=tk.NW, fill=tk.X, padx=5, pady=5)

        frame = ttk.Frame(win1, borderwidth=1, relief=tk.SOLID, padding=[8, 10])
        label = ttk.Label(frame, text='Пароль')
        label.pack(anchor=tk.NW)
        entry_pass = ttk.Entry(frame, show="*") #hide password
        entry_pass.pack(anchor=tk.NW)
        frame.pack(anchor=tk.NW, fill=tk.X, padx=5, pady=5)

        def add_user():
            pers = Person(entry_fio.get(), entry_email.get(), entry_ps.get(), entry_pass.get())
            self.cur.execute("INSERT INTO Users (fio, email, ps, password) VALUES (?, ?, ?, ?)",
                        (pers.fio, pers.email, pers.ps, pers.password))
            self.db.commit()
            self.load_users()
            win1.destroy()

        tk.Button(win1, text='ОК', command=add_user).place(rely=1, width=50, height=30, relx=0.5, anchor=tk.S, y=-20)

    def window_update_user(self):
        win1 = tk.Toplevel(self.master)
        win1.title("Измените данные")
        win1.geometry("250x350")

        # ... (Code for user input frames remains the same)
        frame = ttk.Frame(win1, borderwidth=1, relief=tk.SOLID, padding=[8, 10])
        label = ttk.Label(frame, text='ФИО')
        label.pack(anchor=tk.NW)
        entry_fio = ttk.Entry(frame)
        entry_fio.pack(anchor=tk.NW)
        frame.pack(anchor=tk.NW, fill=tk.X, padx=5, pady=5)

        frame = ttk.Frame(win1, borderwidth=1, relief=tk.SOLID, padding=[8, 10])
        label = ttk.Label(frame, text='Пасп. данные')
        label.pack(anchor=tk.NW)
        entry_ps = ttk.Entry(frame)
        entry_ps.pack(anchor=tk.NW)
        frame.pack(anchor=tk.NW, fill=tk.X, padx=5, pady=5)

        frame = ttk.Frame(win1, borderwidth=1, relief=tk.SOLID, padding=[8, 10])
        label = ttk.Label(frame, text='email')
        label.pack(anchor=tk.NW)
        entry_email = ttk.Entry(frame)
        entry_email.pack(anchor=tk.NW)
        frame.pack(anchor=tk.NW, fill=tk.X, padx=5, pady=5)

        frame = ttk.Frame(win1, borderwidth=1, relief=tk.SOLID, padding=[8, 10])
        label = ttk.Label(frame, text='Пароль')
        label.pack(anchor=tk.NW)
        entry_pass = ttk.Entry(frame, show="*") #hide password
        entry_pass.pack(anchor=tk.NW)
        frame.pack(anchor=tk.NW, fill=tk.X, padx=5, pady=5)

        def update_user():
            selected_item = self.tree.selection()
            if not selected_item:
                showerror(title="Error", message="Please select a user to update.")
                win1.destroy()
                return
            
            user_email = self.tree.item(selected_item[0])["values"][2]
            self.cur.execute('DELETE FROM Users WHERE email = ?', (user_email,))
            pers = Person(entry_fio.get(), entry_email.get(), entry_ps.get(), entry_pass.get())
            self.cur.execute("INSERT INTO Users (fio, email, ps, password) VALUES (?, ?, ?, ?)",
                        (pers.fio, pers.email, pers.ps, pers.password))
            self.db.commit()
            self.load_users()
            win1.destroy()

        tk.Button(win1, text='ОК', command=update_user).place(rely=1, width=50, height=30, relx=0.5, anchor=tk.S, y=-20)
    
    def del_user(self):
        for i in self.tree.selection():
            self.cur.execute('DELETE FROM Users WHERE email = ?', (self.tree.item(i)["values"][2],))
            self.tree.delete(i)
        self.db.commit()

    def add_notes(self):
        note_frame = ttk.Frame(self.notebook1)
        self.notebook1.add(note_frame, text="Новая заметка")

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
            self.notes[title] = content.strip()

            self.cur.execute("INSERT INTO Notes (email, title, content) VALUES (?, ?, ?)",
                        (self.gl_user, title, content))
            self.db.commit()

            note_content = tk.Text(self.notebook1, width=40, height=10)
            note_content.insert(tk.END, content)
            self.notebook1.forget(self.notebook1.select())
            self.notebook1.add(note_content, text=title)

        tk.Button(note_frame, text="ОК", command=save_notes, background="#4682B4",foreground="#FFFFFF").place(x=550, y=-284, width=100, height=40, rely=0, relx=1, anchor=tk.NE)


    def delete_notes(self):
        try:
            current_tab = self.notebook1.index(self.notebook1.select())
            note_title = self.notebook1.tab(current_tab, "text")
            confirm = messagebox.askyesno('Предупреждение', f"Вы действительно хотите удалить {note_title}?")
            if confirm:
                self.notebook1.forget(current_tab)
                self.cur.execute('DELETE FROM Notes WHERE email = ? AND title = ?', (self.gl_user, note_title))
                self.db.commit()
        except tk.TclError:
            pass
        
    def load_notes(self):
        try:
            self.cur.execute('SELECT title, content FROM Notes WHERE email = ?', (self.gl_user,))
            note = self.cur.fetchall()
            for title, content in note:
                note_content = tk.Text(self.notebook1, width=40, height=10)
                note_content.insert(tk.END, content)
                self.notebook1.add(note_content, text=title)
        except Exception as e:
            showerror(title='предупреждение', message=f'Error => {e}')
            
    def __del__(self):
        self.db.close()


if __name__ == "__main__":
    root = tk.Tk()
    noteApp =NoteApp(root)
