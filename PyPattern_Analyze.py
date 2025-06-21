import tkinter as tk
from tkinter import ttk
import json
import colorsys
import os
from collections import defaultdict

tmp = []

ALL_USER_DATA_STRINGS = {}

for i in os.scandir("SolvyJsonsFile"):
    if i.is_file() and i.name.endswith(".json"):
        tmp.append(i.path)
        
        try:
            data = json.load(open(i.path))
        except (json.JSONDecodeError, TypeError) as e:
            print(f"Error decoding JSON for {user_name}: {e}")

    
        ALL_USER_DATA_STRINGS[data["profileName"]] = data

class PuzzleApp:

    def __init__(self, root):
    
        self.root = root
        self.root.title("Multi-User Puzzle Pattern Visualizer")
        self.root.geometry("1000x700")
        self.root.config(bg='black')

        # main
        self.root.grid_rowconfigure(0, weight=1)
        self.root.grid_columnconfigure(0, weight=1, minsize=150)
        self.root.grid_columnconfigure(1, weight=4)
        self.root.grid_columnconfigure(2, weight=1, minsize=150)

        # chap
        user_frame = tk.Frame(self.root, bg='#1a1a1a', relief=tk.RAISED, borderwidth=2)
        user_frame.grid(row=0, column=0, sticky='nsew', padx=10, pady=10)
        tk.Label(user_frame, text="Select User", font=("Helvetica", 16, "bold"), bg='#1a1a1a', fg='white').pack(pady=10)

        # vasat
        center_frame = tk.Frame(self.root, bg='black')
        center_frame.grid(row=0, column=1, sticky='nsew', padx=5, pady=10)
        center_frame.grid_rowconfigure(1, weight=1)
        center_frame.grid_columnconfigure(0, weight=1)
        
        self.active_user_label = tk.Label(center_frame, text="Active User: None", font=("Helvetica", 14), bg='black', fg='white')
        self.active_user_label.grid(row=0, column=0, pady=(0, 5))

        self.canvas = tk.Canvas(center_frame, bg='black', highlightthickness=0)
        self.canvas.grid(row=1, column=0, sticky='nsew')
        self.canvas.bind("<Configure>", self.on_canvas_resize)

        # rast
        level_frame = tk.Frame(self.root, bg='#1a1a1a', relief=tk.RAISED, borderwidth=2)
        level_frame.grid(row=0, column=2, sticky='nsew', padx=10, pady=10)
        tk.Label(level_frame, text="Select Level", font=("Helvetica", 16, "bold"), bg='#1a1a1a', fg='white').pack(pady=10)

        # --- Data and State Variables ---
        self.all_profile_data = {}
        self.level_data_cache = {}
        self._load_all_profile_data()

        self.current_user = None
        self.current_level = 1
        self.current_grid_size = 3
        
        # --- Create User and Level Buttons ---
        self.user_buttons = {}
        user_list = sorted(self.all_profile_data.keys())
        for user_name in user_list:
            button = tk.Button(
                user_frame, text=user_name, font=("Helvetica", 12),
                bg="#333", fg="white", activebackground="#555",
                activeforeground="white", relief=tk.FLAT,
                command=lambda u=user_name: self.select_user(u)
            )
            button.pack(pady=5, padx=10, fill=tk.X)
            self.user_buttons[user_name] = button

        self.level_buttons = []
        for i in range(5):
            level_num = i + 1
            grid_size = i + 3
            button = tk.Button(
                level_frame, text=f"Level {level_num}", font=("Helvetica", 12),
                bg="#333", fg="white", activebackground="#555",
                activeforeground="white", relief=tk.FLAT,
                command=lambda s=grid_size, l=level_num: self.set_puzzle(s, l)
            )
            button.pack(pady=5, padx=10, fill=tk.X)
            self.level_buttons.append(button)

        # --- Initial State ---
        if user_list:
            self.root.after(100, lambda: self.select_user(user_list[0]))

    def _load_all_profile_data(self):
        """Loads and parses all embedded JSON data from the dictionary."""
        for user_name, user_json in ALL_USER_DATA_STRINGS.items():
            #try:
                self.all_profile_data[user_name] = user_json #json.loads(user_json_str)
                self.level_data_cache[user_name] = {}
            #except (json.JSONDecodeError, TypeError) as e:
            #    print(f"Error decoding JSON for {user_name}: {e}")

    def select_user(self, user_name):
        """Sets the active user and updates the visualization to default to Level 1."""
        if user_name not in self.all_profile_data:
            return
        
        self.current_user = user_name
        self.active_user_label.config(text=f"Active User: {user_name}")
        # Default to level 1
        self.set_puzzle(3, 1)

    def _print_debug_info(self, piece_num):
        """Prints the click sequence and timestamps for the current level."""
        print(f"\n--- Debug Info Triggered by Clicking Piece {piece_num} ---")
        
        if not self.current_user:
            print("No user selected.")
            return

        user_data = self.all_profile_data.get(self.current_user)
        if not user_data:
            print(f"No data found for user: {self.current_user}")
            return
            
        level_data = next((lvl for lvl in user_data.get('levels', []) if lvl['levelNumber'] == self.current_level), None)
        if not level_data:
            print(f"No data found for Level {self.current_level}")
            return
        
        click_sequence = level_data.get('clickSequence', [])
        click_events = level_data.get('clickEvents', [])
        
        piece_clicks = [event for event in click_events if not event.startswith('switch')]
        timestamps = [event.split(', ')[1] for event in piece_clicks]

        print(f"User: {self.current_user}, Level: {self.current_level}")
        print(f"Click Sequence ({len(click_sequence)} clicks): {click_sequence}")
        print(f"Timestamps     ({len(timestamps)} times): {timestamps}")
        print("--- End of Debug Info ---")


    def _process_level_data(self, user_name, level_number):
        """Processes the click sequence to find the unique path order."""
        if user_name in self.level_data_cache and level_number in self.level_data_cache[user_name]:
            return

        user_data = self.all_profile_data.get(user_name, {})
        level_data = next((lvl for lvl in user_data.get('levels', []) if lvl['levelNumber'] == level_number), None)
        
        processed_data = {'path_order': {}, 'total_path_length': 0}

        if level_data:
            click_sequence = level_data.get('clickSequence', [])
            
            # Determine the unique path order
            unique_path = []
            for piece_num in click_sequence:
                if piece_num not in unique_path:
                    unique_path.append(piece_num)
            
            path_order = {piece_num: i + 1 for i, piece_num in enumerate(unique_path)}
            
            processed_data['path_order'] = path_order
            processed_data['total_path_length'] = len(unique_path)

        self.level_data_cache.setdefault(user_name, {})[level_number] = processed_data

    def _get_color_for_path(self, order, total_length):
        """Calculates a color based on a linear gradient from light to dark."""
        if total_length <= 1:
            return '#a1ffff'

        # Calculate the interpolation factor (intensity) from 0.0 to 1.0
        intensity = (order - 1) / (total_length - 1)

        # Define the start and end colors in RGB format
        start_r, start_g, start_b = 161, 255, 255  # #a1ffff
        end_r, end_g, end_b = 20, 9, 79            # #14094f

        # Linearly interpolate each color component
        r = int(start_r + (end_r - start_r) * intensity)
        g = int(start_g + (end_g - start_g) * intensity)
        b = int(start_b + (end_b - start_b) * intensity)

        return f'#{r:02x}{g:02x}{b:02x}'


    def set_puzzle(self, size, level):
        """Sets the grid size and level for the current user and redraws."""
        if not self.current_user:
            return
        self.current_grid_size = size
        self.current_level = level
        self._process_level_data(self.current_user, level)
        self.draw_puzzle(size)

    def draw_puzzle(self, size):
        """Draws the puzzle grid based on the click sequence pattern."""
        self.canvas.delete("all")
        if not self.current_user: return

        processed_data = self.level_data_cache.get(self.current_user, {}).get(self.current_level)
        if not processed_data or 'path_order' not in processed_data:
             self.canvas.create_text(self.canvas.winfo_width() / 2, self.canvas.winfo_height() / 2, text=f"No data for Level {self.current_level}", font=("Helvetica", 14), fill="red")
             return
        
        path_order = processed_data.get('path_order', {})
        total_path_length = processed_data.get('total_path_length', 0)
        
        padding, gap = 20, 5
        canvas_width, canvas_height = self.canvas.winfo_width(), self.canvas.winfo_height()
        if canvas_width <= 1 or canvas_height <= 1: return

        puzzle_area_side = min(canvas_width - 2 * padding, canvas_height - 2 * padding)
        tile_size = (puzzle_area_side - (size - 1) * gap) / size

        if tile_size <= 0:
            return

        start_x = (canvas_width - puzzle_area_side) / 2
        start_y = (canvas_height - puzzle_area_side) / 2

        for row in range(size):
            for col in range(size):
                piece_num = row * size + col + 1
                
                path_num = path_order.get(piece_num)
                
                fill_color = self._get_color_for_path(path_num, total_path_length) if path_num is not None else '#FFFFFF'

                x1, y1 = start_x + col * (tile_size + gap), start_y + row * (tile_size + gap)
                x2, y2 = x1 + tile_size, y1 + tile_size

                rect_id = self.canvas.create_rectangle(x1, y1, x2, y2, fill=fill_color, outline='grey30', width=2)
                
                self.canvas.tag_bind(rect_id, '<Button-1>', lambda event, p=piece_num: self._print_debug_info(p))

                if path_num is not None:
                    r, g, b = self.root.winfo_rgb(fill_color)
                    brightness = (r/65535*299 + g/65535*587 + b/65535*114) / 1000
                    text_color = "white" if brightness < 0.5 else "black"
                    
                    self.canvas.create_text(
                        (x1 + x2) / 2, (y1 + y2) / 2,
                        text=str(path_num), 
                        font=("Helvetica", int(tile_size/4), "bold"), 
                        fill=text_color, 
                        anchor='center'
                    )

    def on_canvas_resize(self, event):
        """Redraws the puzzle on resize."""
        if hasattr(self, 'current_grid_size') and self.current_user:
            self.draw_puzzle(self.current_grid_size)

if __name__ == '__main__':
    main_window = tk.Tk()
    app = PuzzleApp(main_window)
    main_window.mainloop()
