# ProjectDataManager file data info

## Данные проекта

### Список проектов

```asm
projects.ini
{
	str: name
	str: path
	loop
}
```

### Данные конкретного проекта

```asm
wdproj.ini
{
	; count: 6
	str: name
	str: path
	str: description (one line)
	str: author
	str: main html page
	str: wdstyle name
}
```

## Файлы-ссылки

### web страница

```asm
*.wdweb
{
	[Flags]
	byte: flags
	{
		html = 1
		md   = 2
		css  = 4
	}
}
```

### глобальны файл для css и html

```asm
*.wdstyle
{
	[Flags]
	byte: flags
	{
		html = 1
		css  = 2
	}
}
```

### контент файл (изображение, текст, тп)

```asm
*.wdcontent
{
	byte: flag
	{
		image = 1
		text = 2
		html = 3
		md = 4
	}
}
```

## Прочее

### byte flag lenght

```asm
max count = 8
[1 2 4 8 16 32 64 128] = 255
```
