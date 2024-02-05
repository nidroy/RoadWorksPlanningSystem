import sys

# Устанавливаем кодировку для stdout и stderr в UTF-8
sys.stdout.reconfigure(encoding='utf-8')
sys.stderr.reconfigure(encoding='utf-8')

arg1 = int(sys.argv[1])
arg2 = int(sys.argv[2])

result = arg1 + arg2

print(result)
