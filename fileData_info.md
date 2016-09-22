# ProjectDataManager file data info

## ������ �������

### ������ ��������

```asm
projects.ini
{
	str: name
	str: path
	loop
}
```

### ������ ����������� �������

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

## �����-������

### web ��������

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

### ��������� ���� ��� css � html

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

### ������� ���� (�����������, �����, ��)

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

## ������

### byte flag lenght

```asm
max count = 8
[1 2 4 8 16 32 64 128] = 255
```
