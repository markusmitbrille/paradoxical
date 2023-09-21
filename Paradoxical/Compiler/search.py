# run this from the solution directory
game_dir = 'C:/Program Files (x86)/Steam/steamapps/common/Crusader Kings III/'
out_dir = './Compiler/'

import os
import re
import glob

def match_and_save_files(path, regex_pattern, output_file):
    # Use a set to store the distinct matches
    distinct_matches = set()

    with open(output_file, 'w') as outfile:
        # Use glob to get a list of all text files in the directory
        text_files = glob.glob(path, recursive=True)

        # Iterate over each text file and process its contents
        for file_path in text_files:
            with open(file_path, 'r', encoding='utf-8-sig') as infile:
                file_contents = infile.read()
              
                # Use regex to find matches in the file contents
                matches = re.findall(regex_pattern, file_contents)
                if matches:
                    # Add the matches to the set
                    distinct_matches.update(matches)

        # Convert the set to a sorted list
        sorted_matches = sorted(distinct_matches)

        # Write the sorted matches to the output file
        for match in sorted_matches:
            outfile.write(f"{match}\n")

# commands
dir = game_dir + 'game/localization/english/**/*.yml'
pattern = r'\bGet\w+'
output = out_dir + 'out_commands.txt'

match_and_save_files(dir, pattern, output)

# animations
dir = game_dir + 'game/events/**/*.txt'
pattern = r'(?<=animation = )\w+'
output = out_dir + 'out_animations.txt'

match_and_save_files(dir, pattern, output)

# themes
dir = game_dir + 'game/events/**/*.txt'
pattern = r'(?<=theme = )\w+'
output = out_dir + 'out_themes.txt'

match_and_save_files(dir, pattern, output)

# outfits
dir = game_dir + 'game/events/**/*.txt'
pattern = r'(?<=outfit_tags = \{ )\w+'
output = out_dir + 'out_outfits.txt'

match_and_save_files(dir, pattern, output)

# pictures
dir = game_dir + 'game/common/decisions/**/*.txt'
pattern = r'(?<=picture = ").+(?=")'
output = out_dir + 'out_pictures.txt'

match_and_save_files(dir, pattern, output)
