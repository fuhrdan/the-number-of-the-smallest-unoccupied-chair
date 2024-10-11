//*****************************************************************************
//** 1942. The Number of the Smallest Unoccupied Chair    leetcode           **
//*****************************************************************************


int compareEvents(const void* a, const void* b)
{
    int* eventA = (int*)a;
    int* eventB = (int*)b;

    if (eventA[0] == eventB[0]) 
    {
        return eventA[1] - eventB[1]; // Prioritize arrival (1) over departure (0) for same time
    }
    return eventA[0] - eventB[0]; // Otherwise, sort by time
}

typedef struct {
    int* data;
    int size;
    int capacity;
} MinHeap;

MinHeap* createMinHeap(int capacity)
{
    MinHeap* heap = (MinHeap*)malloc(sizeof(MinHeap));
    heap->data = (int*)malloc(capacity * sizeof(int));
    heap->size = 0;
    heap->capacity = capacity;
    return heap;
}

void minHeapPush(MinHeap* heap, int value)
{
    heap->data[heap->size++] = value;
    int i = heap->size - 1;
    
    while (i > 0 && heap->data[i] < heap->data[(i - 1) / 2]) 
    {
        int temp = heap->data[i];
        heap->data[i] = heap->data[(i - 1) / 2];
        heap->data[(i - 1) / 2] = temp;
        i = (i - 1) / 2;
    }
}

int minHeapPop(MinHeap* heap)
{
    if (heap->size == 0) return -1;
    int result = heap->data[0];
    heap->data[0] = heap->data[--heap->size];
    
    int i = 0;
    while (2 * i + 1 < heap->size) 
    {
        int j = 2 * i + 1;
        if (j + 1 < heap->size && heap->data[j + 1] < heap->data[j])
            j++;
        if (heap->data[i] <= heap->data[j]) break;
        int temp = heap->data[i];
        heap->data[i] = heap->data[j];
        heap->data[j] = temp;
        i = j;
    }
    
    return result;
}

int smallestChair(int** times, int timesSize, int* timesColSize, int targetFriend)
{
    int events[2 * timesSize][3]; 
    int idx = 0;
    for (int i = 0; i < timesSize; i++) 
    {
        events[idx][0] = times[i][0]; // Arrival time
        events[idx][1] = 1; // Arrival event
        events[idx][2] = i; // Friend index
        idx++;
        events[idx][0] = times[i][1]; // Departure time
        events[idx][1] = 0; // Departure event
        events[idx][2] = i; // Friend index
        idx++;
    }
    
    // Sort the events array
    qsort(events, 2 * timesSize, sizeof(events[0]), compareEvents);

    int* lookup = (int*)malloc(timesSize * sizeof(int));
    
    MinHeap* minHeap = createMinHeap(timesSize);

    for (int i = 0; i < timesSize; i++) 
    {
        minHeapPush(minHeap, i); // Initially, all chairs are available
    }

    for (int i = 0; i < 2 * timesSize; i++) 
    {
        int friendIndex = events[i][2];
        if (events[i][1] == 0) // Departure event
        {
            minHeapPush(minHeap, lookup[friendIndex]); // Add the chair back to the heap
        } 
        else // Arrival event
        {
            int chair = minHeapPop(minHeap); // Assign the smallest available chair
            lookup[friendIndex] = chair;

            if (friendIndex == targetFriend) // Check if it's the target friend
            {
                free(minHeap->data);
                free(minHeap);
                free(lookup);
                return chair;
            }
        }
    }

    free(minHeap->data);
    free(minHeap);
    free(lookup);
    return -1;
}