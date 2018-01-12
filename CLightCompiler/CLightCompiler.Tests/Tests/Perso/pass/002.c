int a() {
	return 3;
}

int b(int x, int y, int z) {
	return x * y + z - a();
}

int main() {
	out b(a(), a(), 2);
}