int a() {
	int ptr;
	ptr = 0;
	*ptr = -1;
}

int b(int p) {
	int ptr;
	ptr = 0;
	out ptr;
	out *ptr;
}

int main() {
	a();
	b(2);
}