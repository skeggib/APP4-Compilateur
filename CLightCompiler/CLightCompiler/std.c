int init() {
	int ptr;
	ptr = 0;
	*ptr = -1;
}

int alloc(int size) {
	int ptr;
	ptr = 0;
	
	while (*ptr != -1) {
		ptr = *ptr;
	}

	int next;
	next = ptr + size + 2;
	*next = -1;

	*ptr = next;

	ptr = ptr + 1;
	*ptr = size;

	return ptr + 1;
}

int free(int addr) {
	
}

int defragment() {

}